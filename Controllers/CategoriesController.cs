using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;

namespace OilShopManagement.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() =>
        View(await _context.Categories.OrderBy(c => c.Name).ToListAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "طھظ… ط¥ط¶ط§ظپط© ط§ظ„طھطµظ†ظٹظپ ط¨ظ†ط¬ط§ط­";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cat = await _context.Categories.FindAsync(id);
        if (cat == null) return NotFound();
        return View(cat);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "طھظ… طھط¹ط¯ظٹظ„ ط§ظ„طھطµظ†ظٹظپ ط¨ظ†ط¬ط§ط­";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _context.Categories.FindAsync(id);
        if (cat == null) return NotFound();
        cat.IsActive = false;
        await _context.SaveChangesAsync();
        TempData["Success"] = "طھظ… ط­ط°ظپ ط§ظ„طھطµظ†ظٹظپ";
        return RedirectToAction(nameof(Index));
    }
}

