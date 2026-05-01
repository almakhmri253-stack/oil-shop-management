using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;
using OilShopManagement.Repositories;

namespace OilShopManagement.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductRepository _products;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    private readonly IWebHostEnvironment _env;

    public ProductsController(IProductRepository products, ApplicationDbContext context, ILogger<ProductsController> logger, IWebHostEnvironment env)
    {
        _products = products;
        _context = context;
        _logger = logger;
        _env = env;
    }

    private async Task<string?> SaveImageAsync(IFormFile? file, string? oldPath = null)
    {
        if (file == null || file.Length == 0) return oldPath;
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowed.Contains(ext)) return oldPath;
        var dir = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(dir);
        if (!string.IsNullOrEmpty(oldPath))
        {
            var old = Path.Combine(_env.WebRootPath, oldPath.TrimStart('/'));
            if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
        }
        var fileName = $"{Guid.NewGuid()}{ext}";
        using var stream = new FileStream(Path.Combine(dir, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/images/products/{fileName}";
    }

    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        IEnumerable<Product> products;

        if (!string.IsNullOrWhiteSpace(search))
            products = await _products.SearchAsync(search);
        else if (categoryId.HasValue)
            products = await _products.GetByCategoryAsync(categoryId.Value);
        else
            products = await _products.GetAllWithCategoryAsync();

        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        var cats = await _context.Categories.Where(c => c.IsActive).ToListAsync();
        ViewBag.Categories = new SelectList(cats, "Id", "Name");
        ViewBag.AllCategories = cats;
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _products.GetWithCategoryAsync(id);
        if (product == null) return NotFound();

        var transactions = await _context.StockTransactions
            .Where(st => st.ProductId == id)
            .OrderByDescending(st => st.CreatedAt).Take(20).ToListAsync();

        ViewBag.Transactions = transactions;
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int? catId, string? grade)
    {
        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", catId);
        var model = new Product();
        if (!string.IsNullOrEmpty(grade)) model.Name = grade;
        if (catId.HasValue) model.CategoryId = catId.Value;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                product.ImagePath = await SaveImageAsync(imageFile);
                await _products.AddAsync(product);
                await _products.SaveChangesAsync();
                TempData["Success"] = "طھظ… ط¥ط¶ط§ظپط© ط§ظ„ظ…ظ†طھط¬ ط¨ظ†ط¬ط§ط­";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError("", "ط­ط¯ط« ط®ط·ط£ ط£ط«ظ†ط§ط، ط§ظ„ط­ظپط¸");
            }
        }
        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null) return NotFound();
        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
    {
        if (id != product.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                product.UpdatedAt = DateTime.UtcNow;
                product.ImagePath = await SaveImageAsync(imageFile, product.ImagePath);
                _products.Update(product);
                await _products.SaveChangesAsync();
                TempData["Success"] = "طھظ… طھط¹ط¯ظٹظ„ ط§ظ„ظ…ظ†طھط¬ ط¨ظ†ط¬ط§ط­";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing product {Id}", id);
                ModelState.AddModelError("", "ط­ط¯ط« ط®ط·ط£ ط£ط«ظ†ط§ط، ط§ظ„ط­ظپط¸");
            }
        }
        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null) return NotFound();

        product.IsActive = false;
        _products.Update(product);
        await _products.SaveChangesAsync();
        TempData["Success"] = "طھظ… ط­ط°ظپ ط§ظ„ظ…ظ†طھط¬ ط¨ظ†ط¬ط§ط­";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetProductJson(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Json(new { product.Id, product.Name, product.SalePrice, product.PurchasePrice, product.CurrentStock, product.Unit });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJson()
    {
        var products = await _products.GetAllWithCategoryAsync();
        return Json(products.Select(p => new { p.Id, p.Name, p.SKU, p.SalePrice, p.PurchasePrice, p.CurrentStock, p.Unit, p.MinStockLevel, p.Brand, p.PiecesPerUnit, p.LitersPerPiece }));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        var products = await _products.SearchAsync(term ?? "");
        return Json(products.Select(p => new { p.Id, p.Name, p.SKU, p.SalePrice, p.PurchasePrice, p.CurrentStock, p.Unit, p.MinStockLevel, p.Brand, p.PiecesPerUnit, p.LitersPerPiece }));
    }
}

