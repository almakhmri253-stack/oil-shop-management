using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;

namespace OilShopManagement.Controllers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly ApplicationDbContext _context;

    public SuppliersController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Suppliers.Where(s => s.IsActive).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.Contains(search) || (s.Phone != null && s.Phone.Contains(search)));

        ViewBag.Search = search;
        return View(await query.OrderBy(s => s.Name).ToListAsync());
    }

    public IActionResult Create() => View(new Supplier());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        if (!ModelState.IsValid) return View(supplier);
        supplier.CreatedAt = DateTime.UtcNow;
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        TempData["Success"] = "طھظ… ط¥ط¶ط§ظپط© ط§ظ„ظ…ظˆط±ط¯ ط¨ظ†ط¬ط§ط­";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return NotFound();
        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supplier supplier)
    {
        if (id != supplier.Id) return NotFound();
        if (!ModelState.IsValid) return View(supplier);
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
        TempData["Success"] = "طھظ… طھط¹ط¯ظٹظ„ ط§ظ„ظ…ظˆط±ط¯ ط¨ظ†ط¬ط§ط­";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return NotFound();
        supplier.IsActive = false;
        await _context.SaveChangesAsync();
        TempData["Success"] = "طھظ… ط­ط°ظپ ط§ظ„ظ…ظˆط±ط¯";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Search(string term)
    {
        var suppliers = await _context.Suppliers
            .Where(s => s.IsActive && (s.Name.Contains(term) || (s.Phone != null && s.Phone.Contains(term))))
            .OrderBy(s => s.Name)
            .Take(10)
            .Select(s => new { s.Id, s.Name, s.Phone, s.ContactPerson })
            .ToListAsync();
        return Json(suppliers);
    }
}


