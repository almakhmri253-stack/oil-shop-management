using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;
using OilShopManagement.ViewModels;
using System.Security.Claims;

namespace OilShopManagement.Controllers;

[Authorize]
public class PurchaseReturnsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PurchaseReturnsController> _logger;

    public PurchaseReturnsController(ApplicationDbContext context, ILogger<PurchaseReturnsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.Today.AddDays(-30);
        var toDate = (to ?? DateTime.Today).AddDays(1);

        var returns = await _context.PurchaseReturns
            .Include(r => r.Supplier)
            .Include(r => r.Purchase)
            .Where(r => r.ReturnDate >= fromDate && r.ReturnDate < toDate)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync();

        ViewBag.From = fromDate.ToString("yyyy-MM-dd");
        ViewBag.To = toDate.AddDays(-1).ToString("yyyy-MM-dd");
        return View(returns);
    }

    public async Task<IActionResult> Details(int id)
    {
        var ret = await _context.PurchaseReturns
            .Include(r => r.Supplier)
            .Include(r => r.Purchase)
            .Include(r => r.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (ret == null) return NotFound();
        return View(ret);
    }

    public async Task<IActionResult> Create(int? purchaseId)
    {
        ViewBag.PurchaseId = purchaseId;
        if (purchaseId.HasValue)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.Id == purchaseId.Value);
            ViewBag.Purchase = purchase;
        }
        return View(new PurchaseReturnCreateViewModel { PurchaseId = purchaseId, ReturnDate = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] PurchaseReturnCreateViewModel vm)
    {
        if (!vm.Items.Any() && (vm.ManualTotal == null || vm.ManualTotal <= 0))
            return BadRequest(new { message = "ظٹط¬ط¨ ط¥ط¶ط§ظپط© ظ…ظ†طھط¬ ط£ظˆ ط¥ط¯ط®ط§ظ„ ط§ظ„ظ…ط¨ظ„ط؛" });

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var ret = new PurchaseReturn
            {
                ReturnNumber = await GenerateReturnNumberAsync(),
                PurchaseId = vm.PurchaseId,
                SupplierId = vm.SupplierId,
                ReturnDate = vm.ReturnDate,
                Reason = vm.Reason,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.UtcNow
            };

            decimal total = 0;
            foreach (var item in vm.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null) return BadRequest(new { message = "ط§ظ„ظ…ظ†طھط¬ ط؛ظٹط± ظ…ظˆط¬ظˆط¯" });
                if (product.CurrentStock < item.Quantity)
                    return BadRequest(new { message = $"ط§ظ„ظƒظ…ظٹط© ط§ظ„ظ…طھظˆظپط±ط© ظ…ظ† {product.Name} ظ‡ظٹ {product.CurrentStock} ظپظ‚ط·" });

                var retItem = new PurchaseReturnItem
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
                product.CurrentStock -= item.Quantity;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = product.Id,
                    Type = TransactionType.Return,
                    Quantity = item.Quantity,
                    QuantityBefore = before,
                    QuantityAfter = product.CurrentStock,
                    UnitPrice = item.UnitPrice,
                    Notes = $"ظ…ط±طھط¬ط¹ ط´ط±ط§ط، {ret.ReturnNumber}",
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedAt = DateTime.UtcNow
                });
                _context.Products.Update(product);
            }

            ret.TotalAmount = (vm.ManualTotal.HasValue && vm.ManualTotal.Value > 0) ? vm.ManualTotal.Value : total;
            _context.PurchaseReturns.Add(ret);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { returnId = ret.Id, returnNumber = ret.ReturnNumber });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating purchase return");
            return StatusCode(500, new { message = "ط­ط¯ط« ط®ط·ط£ ط£ط«ظ†ط§ط، ط¥ظ†ط´ط§ط، ط§ظ„ظ…ط±طھط¬ط¹" });
        }
    }

    private async Task<string> GenerateReturnNumberAsync()
    {
        var today = DateTime.Today;
        var count = await _context.PurchaseReturns.CountAsync(r => r.ReturnDate.Date == today);
        return $"PR-{today:yyyyMMdd}-{(count + 1):D4}";
    }
}

