using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OilShopManagement.Models;
using OilShopManagement.Repositories;
using OilShopManagement.ViewModels;
using System.Security.Claims;

namespace OilShopManagement.Controllers;

[Authorize]
public class StockController : Controller
{
    private readonly IStockRepository _stock;
    private readonly IProductRepository _products;

    public StockController(IStockRepository stock, IProductRepository products)
    {
        _stock = stock;
        _products = products;
    }

    public async Task<IActionResult> Index()
    {
        var transactions = await _stock.GetRecentAsync(50);
        return View(transactions);
    }

    public async Task<IActionResult> Product(int id)
    {
        var product = await _products.GetWithCategoryAsync(id);
        if (product == null) return NotFound();
        var transactions = await _stock.GetByProductAsync(id);
        ViewBag.Product = product;
        return View(transactions);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Adjust()
    {
        var vm = new StockAdjustViewModel
        {
            Products = new SelectList(await _products.GetAllWithCategoryAsync(), "Id", "Name")
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Adjust(StockAdjustViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var product = await _products.GetByIdAsync(vm.ProductId);
            if (product == null) return NotFound();

            var before = product.CurrentStock;
            if (vm.Type == TransactionType.Purchase || vm.Type == TransactionType.Adjustment || vm.Type == TransactionType.Return)
                product.CurrentStock += vm.Quantity;
            else
                product.CurrentStock -= vm.Quantity;

            if (product.CurrentStock < 0)
            {
                ModelState.AddModelError("Quantity", "ط§ظ„ظƒظ…ظٹط© ظ„ط§ ظٹظ…ظƒظ† ط£ظ† طھظƒظˆظ† ط£ظ‚ظ„ ظ…ظ† طµظپط±");
                vm.Products = new SelectList(await _products.GetAllWithCategoryAsync(), "Id", "Name");
                return View(vm);
            }

            await _stock.AddAsync(new StockTransaction
            {
                ProductId = product.Id,
                Type = vm.Type,
                Quantity = vm.Quantity,
                QuantityBefore = before,
                QuantityAfter = product.CurrentStock,
                UnitPrice = vm.UnitPrice,
                Notes = vm.Notes,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.UtcNow
            });
            _products.Update(product);
            await _stock.SaveChangesAsync();

            TempData["Success"] = "طھظ… طھط¹ط¯ظٹظ„ ط§ظ„ظ…ط®ط²ظˆظ† ط¨ظ†ط¬ط§ط­";
            return RedirectToAction(nameof(Index));
        }
        vm.Products = new SelectList(await _products.GetAllWithCategoryAsync(), "Id", "Name");
        return View(vm);
    }
}


