using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;
using OilShopManagement.ViewModels;
using System.Security.Claims;

namespace OilShopManagement.Controllers;

[Authorize]
public class PurchasesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PurchasesController> _logger;

    public PurchasesController(ApplicationDbContext context, ILogger<PurchasesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = (to ?? DateTime.UtcNow.Date).AddDays(1);

        var query = _context.Purchases
            .Include(p => p.Supplier)
            .Where(p => p.PurchaseDate >= fromDate && p.PurchaseDate < toDate)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.PurchaseNumber.Contains(search)
                || (p.Supplier != null && p.Supplier.Name.Contains(search)));

        ViewBag.From = fromDate.ToString("yyyy-MM-dd");
        ViewBag.To = toDate.AddDays(-1).ToString("yyyy-MM-dd");
        ViewBag.Search = search;
        return View(await query.OrderByDescending(p => p.PurchaseDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null) return NotFound();
        return View(purchase);
    }

    public async Task<IActionResult> Print(int id)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null) return NotFound();
        return View(purchase);
    }

    [HttpGet]
    public async Task<IActionResult> GetByNumber(string number)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.PurchaseNumber == number);
        if (purchase == null) return NotFound(new { message = "ظپط§طھظˆط±ط© ط§ظ„ط´ط±ط§ط، ط؛ظٹط± ظ…ظˆط¬ظˆط¯ط©" });
        return Json(new
        {
            id = purchase.Id,
            purchaseNumber = purchase.PurchaseNumber,
            supplierName = purchase.Supplier?.Name,
            purchaseDate = purchase.PurchaseDate.ToString("yyyy-MM-dd"),
            totalAmount = purchase.TotalAmount,
            items = purchase.Items.Select(i => new
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
        var purchases = await _context.Purchases
            .Include(p => p.Supplier)
            .Where(p => p.PurchaseNumber.Contains(term) || (p.Supplier != null && p.Supplier.Name.Contains(term)))
            .OrderByDescending(p => p.PurchaseDate)
            .Take(8)
            .Select(p => new { p.Id, p.PurchaseNumber, supplierName = p.Supplier != null ? p.Supplier.Name : "" })
            .ToListAsync();
        return Json(purchases);
    }

    public IActionResult Create()
    {
        return View(new PurchaseCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] PurchaseCreateViewModel vm)
    {
        if (!vm.Items.Any() && (vm.ManualSubTotal == null || vm.ManualSubTotal <= 0))
            return BadRequest(new { message = "ظٹط¬ط¨ ط¥ط¶ط§ظپط© ظ…ظ†طھط¬ ط£ظˆ ط¥ط¯ط®ط§ظ„ ط§ظ„ظ…ط¨ظ„ط؛" });

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var purchase = new Purchase
            {
                PurchaseNumber = await GeneratePurchaseNumberAsync(),
                SupplierId = vm.SupplierId,
                PurchaseDate = vm.PurchaseDate,
                PaymentMethod = vm.PaymentMethod,
                TaxRate = vm.TaxRate,
                Discount = vm.Discount,
                Notes = vm.Notes,
                Status = PurchaseStatus.Completed,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.UtcNow
            };

            decimal subTotal = 0;
            foreach (var item in vm.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null) return BadRequest(new { message = "ط§ظ„ظ…ظ†طھط¬ ط؛ظٹط± ظ…ظˆط¬ظˆط¯" });

                var purchaseItem = new PurchaseItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.UnitPrice * item.Quantity
                };
                purchase.Items.Add(purchaseItem);
                subTotal += purchaseItem.Total;

                // Carton products: item.Quantity is in pieces, stock is in cartons
                bool isCarton = item.PiecesPerUnit > 0;
                int cartonsToAdd = isCarton
                    ? (int)Math.Ceiling((double)item.Quantity / item.PiecesPerUnit)
                    : item.Quantity;

                var before = product.CurrentStock;
                product.CurrentStock += cartonsToAdd;
                product.PurchasePrice = item.UnitPrice; // piece price
                if (isCarton) product.PiecesPerUnit = item.PiecesPerUnit;

                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = product.Id,
                    Type = TransactionType.Purchase,
                    Quantity = cartonsToAdd,
                    QuantityBefore = before,
                    QuantityAfter = product.CurrentStock,
                    UnitPrice = item.UnitPrice,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedAt = DateTime.UtcNow
                });
                _context.Products.Update(product);
            }

            var finalSubTotal = (vm.ManualSubTotal.HasValue && vm.ManualSubTotal.Value > 0)
                ? vm.ManualSubTotal.Value : subTotal;
            purchase.SubTotal = finalSubTotal;
            purchase.TaxAmount = finalSubTotal * (purchase.TaxRate / 100);
            purchase.TotalAmount = finalSubTotal + purchase.TaxAmount - purchase.Discount;

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { purchaseId = purchase.Id, purchaseNumber = purchase.PurchaseNumber });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating purchase");
            return StatusCode(500, new { message = "ط­ط¯ط« ط®ط·ط£ ط£ط«ظ†ط§ط، ط¥ظ†ط´ط§ط، ظپط§طھظˆط±ط© ط§ظ„ط´ط±ط§ط،" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cancel(int id)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null) return NotFound();
        if (purchase.Status == PurchaseStatus.Cancelled)
        {
            TempData["Error"] = "ظپط§طھظˆط±ط© ط§ظ„ط´ط±ط§ط، ظ…ظ„ط؛ط§ط© ط¨ط§ظ„ظپط¹ظ„";
            return RedirectToAction(nameof(Details), new { id });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            purchase.Status = PurchaseStatus.Cancelled;
            foreach (var item in purchase.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    var before = product.CurrentStock;
                    product.CurrentStock = Math.Max(0, product.CurrentStock - item.Quantity);
                    _context.StockTransactions.Add(new StockTransaction
                    {
                        ProductId = product.Id,
                        Type = TransactionType.Adjustment,
                        Quantity = item.Quantity,
                        QuantityBefore = before,
                        QuantityAfter = product.CurrentStock,
                        Notes = $"ط¥ظ„ط؛ط§ط، ظپط§طھظˆط±ط© ط´ط±ط§ط، {purchase.PurchaseNumber}",
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.Products.Update(product);
                }
            }
            _context.Purchases.Update(purchase);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["Success"] = "طھظ… ط¥ظ„ط؛ط§ط، ظپط§طھظˆط±ط© ط§ظ„ط´ط±ط§ط، ظˆطھط¹ط¯ظٹظ„ ط§ظ„ظ…ط®ط²ظˆظ†";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error cancelling purchase {Id}", id);
            TempData["Error"] = "ط­ط¯ط« ط®ط·ط£ ط£ط«ظ†ط§ط، ط§ظ„ط¥ظ„ط؛ط§ط،";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<string> GeneratePurchaseNumberAsync()
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.Purchases.CountAsync(p => p.PurchaseDate.Date == today);
        return $"PUR-{today:yyyyMMdd}-{(count + 1):D4}";
    }
}


