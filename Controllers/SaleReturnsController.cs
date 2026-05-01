using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;
using OilShopManagement.ViewModels;
using System.Security.Claims;

namespace OilShopManagement.Controllers;

[Authorize]
public class SaleReturnsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SaleReturnsController> _logger;

    public SaleReturnsController(ApplicationDbContext context, ILogger<SaleReturnsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(DateTime? from, DateTime? to, string? search)
    {
        var fromDate = from ?? DateTime.Today.AddDays(-30);
        var toDate = (to ?? DateTime.Today).AddDays(1);

        var query = _context.SaleReturns
            .Include(r => r.Customer)
            .Include(r => r.Invoice)
            .Where(r => r.ReturnDate >= fromDate && r.ReturnDate < toDate)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.ReturnNumber.Contains(search)
                || (r.Customer != null && r.Customer.Name.Contains(search))
                || (r.Invoice != null && r.Invoice.InvoiceNumber.Contains(search)));

        ViewBag.From = fromDate.ToString("yyyy-MM-dd");
        ViewBag.To = toDate.AddDays(-1).ToString("yyyy-MM-dd");
        ViewBag.Search = search;
        return View(await query.OrderByDescending(r => r.ReturnDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var ret = await _context.SaleReturns
            .Include(r => r.Customer)
            .Include(r => r.Invoice)
            .Include(r => r.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (ret == null) return NotFound();
        return View(ret);
    }

    public async Task<IActionResult> Create(int? invoiceId)
    {
        ViewBag.InvoiceId = invoiceId;
        if (invoiceId.HasValue)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Items).ThenInclude(ii => ii.Product)
                .FirstOrDefaultAsync(i => i.Id == invoiceId.Value);
            ViewBag.Invoice = invoice;
        }
        return View(new SaleReturnCreateViewModel { InvoiceId = invoiceId, ReturnDate = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] SaleReturnCreateViewModel vm)
    {
        if (!vm.Items.Any() && (vm.ManualTotal == null || vm.ManualTotal <= 0))
            return BadRequest(new { message = "يجب إضافة منتج أو إدخال المبلغ" });

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var ret = new SaleReturn
            {
                ReturnNumber = await GenerateReturnNumberAsync(),
                InvoiceId = vm.InvoiceId,
                CustomerId = vm.CustomerId,
                ReturnDate = vm.ReturnDate,
                Reason = vm.Reason,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now
            };

            decimal total = 0;
            foreach (var item in vm.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null) return BadRequest(new { message = "المنتج غير موجود" });

                var retItem = new SaleReturnItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.UnitPrice * item.Quantity
                };
                ret.Items.Add(retItem);
                total += retItem.Total;

                var before = product.CurrentStock;
                product.CurrentStock += item.Quantity;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = product.Id,
                    Type = TransactionType.Return,
                    Quantity = item.Quantity,
                    QuantityBefore = before,
                    QuantityAfter = product.CurrentStock,
                    UnitPrice = item.UnitPrice,
                    Notes = $"مرتجع مبيعات {ret.ReturnNumber}",
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedAt = DateTime.Now
                });
                _context.Products.Update(product);
            }

            ret.TotalAmount = (vm.ManualTotal.HasValue && vm.ManualTotal.Value > 0) ? vm.ManualTotal.Value : total;
            _context.SaleReturns.Add(ret);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { returnId = ret.Id, returnNumber = ret.ReturnNumber });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating sale return");
            return StatusCode(500, new { message = "حدث خطأ أثناء إنشاء المرتجع" });
        }
    }

    private async Task<string> GenerateReturnNumberAsync()
    {
        var today = DateTime.Today;
        var count = await _context.SaleReturns.CountAsync(r => r.ReturnDate.Date == today);
        return $"SR-{today:yyyyMMdd}-{(count + 1):D4}";
    }
}
