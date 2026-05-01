using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;
using OilShopManagement.Repositories;
using OilShopManagement.ViewModels;
using System.Security.Claims;

namespace OilShopManagement.Controllers;

[Authorize]
public class InvoicesController : Controller
{
    private readonly IInvoiceRepository _invoices;
    private readonly IProductRepository _products;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IInvoiceRepository invoices, IProductRepository products,
        ApplicationDbContext context, ILogger<InvoicesController> logger)
    {
        _invoices = invoices;
        _products = products;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.Today.AddDays(-30);
        var toDate = (to ?? DateTime.Today).AddDays(1);

        var invoices = await _invoices.GetByDateRangeAsync(fromDate, toDate);

        if (!string.IsNullOrWhiteSpace(search))
            invoices = invoices.Where(i => i.InvoiceNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (i.Customer?.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

        ViewBag.From = fromDate.ToString("yyyy-MM-dd");
        ViewBag.To = toDate.AddDays(-1).ToString("yyyy-MM-dd");
        ViewBag.Search = search;
        return View(invoices);
    }

    public async Task<IActionResult> Details(int id)
    {
        var invoice = await _invoices.GetWithDetailsAsync(id);
        if (invoice == null) return NotFound();
        return View(invoice);
    }

    public async Task<IActionResult> Print(int id)
    {
        var invoice = await _invoices.GetWithDetailsAsync(id);
        if (invoice == null) return NotFound();
        return View(invoice);
    }

    [HttpGet]
    public async Task<IActionResult> GetByNumber(string number)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == number);
        if (invoice == null) return NotFound(new { message = "الفاتورة غير موجودة" });
        return Json(new
        {
            id = invoice.Id,
            invoiceNumber = invoice.InvoiceNumber,
            customerName = invoice.Customer?.Name,
            invoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
            totalAmount = invoice.TotalAmount,
            items = invoice.Items.Select(i => new
            {
                productId = i.ProductId,
                productName = i.ProductName,
                quantity = i.Quantity,
                unitPrice = i.UnitPrice,
                total = i.Total
            })
        });
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Customer)
            .Where(i => i.InvoiceNumber.Contains(term) || (i.Customer != null && i.Customer.Name.Contains(term)))
            .OrderByDescending(i => i.InvoiceDate)
            .Take(8)
            .Select(i => new { i.Id, i.InvoiceNumber, customerName = i.Customer != null ? i.Customer.Name : "" })
            .ToListAsync();
        return Json(invoices);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new InvoiceCreateViewModel
        {
            Customers = new SelectList(await _context.Customers.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(), "Id", "Name"),
            Products = new SelectList(await _products.GetAllWithCategoryAsync(), "Id", "Name")
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] InvoiceCreateViewModel vm)
    {
        if (!vm.Items.Any() && (vm.ManualSubTotal == null || vm.ManualSubTotal <= 0))
            return BadRequest(new { message = "يجب إضافة منتج أو إدخال المبلغ" });

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var invoice = new Invoice
            {
                InvoiceNumber = await _invoices.GenerateInvoiceNumberAsync(),
                CustomerId = vm.CustomerId,
                InvoiceDate = vm.InvoiceDate,
                PaymentMethod = vm.PaymentMethod,
                TaxRate = vm.TaxRate,
                Discount = vm.Discount,
                Notes = vm.Notes,
                Status = InvoiceStatus.Completed,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now
            };

            decimal subTotal = 0;
            foreach (var item in vm.Items)
            {
                var product = await _products.GetByIdAsync(item.ProductId);
                if (product == null) return BadRequest(new { message = $"المنتج غير موجود" });

                // For carton products: item.Quantity is in pieces, stock is in cartons
                bool isCarton = (product.Unit == "كرتون" || product.Unit == "Carton")
                                && product.PiecesPerUnit.HasValue && product.PiecesPerUnit > 0;
                int cartonsNeeded = isCarton
                    ? (int)Math.Ceiling((double)item.Quantity / product.PiecesPerUnit!.Value)
                    : item.Quantity;

                if (product.CurrentStock < cartonsNeeded)
                {
                    var available = isCarton
                        ? product.CurrentStock * product.PiecesPerUnit!.Value
                        : product.CurrentStock;
                    return BadRequest(new { message = $"الكمية المتوفرة من {product.Name} هي {available} فقط" });
                }

                var invoiceItem = new InvoiceItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice > 0 ? item.UnitPrice : product.SalePrice,
                    PurchasePrice = product.PurchasePrice,
                    Total = (item.UnitPrice > 0 ? item.UnitPrice : product.SalePrice) * item.Quantity
                };
                invoice.Items.Add(invoiceItem);
                subTotal += invoiceItem.Total;

                // Update stock (carton products deduct by carton count)
                var stockBefore = product.CurrentStock;
                product.CurrentStock -= cartonsNeeded;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = product.Id,
                    Type = TransactionType.Sale,
                    Quantity = cartonsNeeded,
                    QuantityBefore = stockBefore,
                    QuantityAfter = product.CurrentStock,
                    UnitPrice = invoiceItem.UnitPrice,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedAt = DateTime.Now
                });
                _context.Products.Update(product);
            }

            var finalSubTotal = (vm.ManualSubTotal.HasValue && vm.ManualSubTotal.Value > 0)
                ? vm.ManualSubTotal.Value : subTotal;
            invoice.SubTotal = finalSubTotal;
            invoice.TaxAmount = finalSubTotal * (invoice.TaxRate / 100);
            invoice.TotalAmount = finalSubTotal + invoice.TaxAmount - invoice.Discount;

            await _invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { invoiceId = invoice.Id, invoiceNumber = invoice.InvoiceNumber });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating invoice");
            return StatusCode(500, new { message = "حدث خطأ أثناء إنشاء الفاتورة" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cancel(int id)
    {
        var invoice = await _invoices.GetWithDetailsAsync(id);
        if (invoice == null) return NotFound();
        if (invoice.Status == InvoiceStatus.Cancelled)
        {
            TempData["Error"] = "الفاتورة ملغاة بالفعل";
            return RedirectToAction(nameof(Details), new { id });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            invoice.Status = InvoiceStatus.Cancelled;
            foreach (var item in invoice.Items)
            {
                var product = await _products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    var before = product.CurrentStock;
                    product.CurrentStock += item.Quantity;
                    _context.StockTransactions.Add(new StockTransaction
                    {
                        ProductId = product.Id,
                        Type = TransactionType.Return,
                        Quantity = item.Quantity,
                        QuantityBefore = before,
                        QuantityAfter = product.CurrentStock,
                        Notes = $"إلغاء فاتورة {invoice.InvoiceNumber}",
                        InvoiceId = invoice.Id
                    });
                    _context.Products.Update(product);
                }
            }
            _invoices.Update(invoice);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["Success"] = "تم إلغاء الفاتورة وإعادة المخزون";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error cancelling invoice {Id}", id);
            TempData["Error"] = "حدث خطأ أثناء الإلغاء";
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}
