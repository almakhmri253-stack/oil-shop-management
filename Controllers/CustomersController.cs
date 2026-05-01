using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;

namespace OilShopManagement.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ApplicationDbContext context, ILogger<CustomersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Customers.Where(c => c.IsActive).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search) || (c.Phone != null && c.Phone.Contains(search)));

        ViewBag.Search = search;
        return View(await query.OrderBy(c => c.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Invoices.OrderByDescending(i => i.InvoiceDate).Take(10))
            .FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();
        return View(customer);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            customer.CreatedAt = DateTime.Now;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            TempData["Success"] = "تم إضافة العميل بنجاح";
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            TempData["Success"] = "تم تعديل العميل بنجاح";
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        customer.IsActive = false;
        await _context.SaveChangesAsync();
        TempData["Success"] = "تم حذف العميل";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        var customers = await _context.Customers
            .Where(c => c.IsActive && (c.Name.Contains(term) || (c.Phone != null && c.Phone.Contains(term))))
            .Select(c => new { c.Id, c.Name, c.Phone, c.CarModel, c.PlateNumber })
            .Take(10).ToListAsync();
        return Json(customers);
    }
}
