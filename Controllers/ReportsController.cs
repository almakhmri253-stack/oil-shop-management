using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Repositories;
using OilShopManagement.ViewModels;

namespace OilShopManagement.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IInvoiceRepository _invoices;
    private readonly IProductRepository _products;
    private readonly ApplicationDbContext _context;

    public ReportsController(IInvoiceRepository invoices, IProductRepository products, ApplicationDbContext context)
    {
        _invoices = invoices;
        _products = products;
        _context = context;
    }

    public async Task<IActionResult> Sales(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = (to ?? DateTime.UtcNow.Date).AddDays(1);

        var vm = new SalesReportViewModel
        {
            FromDate = fromDate,
            ToDate = toDate.AddDays(-1),
            Invoices = await _invoices.GetByDateRangeAsync(fromDate, toDate),
            TotalSales = await _invoices.GetTotalSalesAsync(fromDate, toDate),
            TotalProfit = await _invoices.GetTotalProfitAsync(fromDate, toDate)
        };
        vm.TotalInvoices = vm.Invoices.Count();
        return View(vm);
    }

    public async Task<IActionResult> Stock()
    {
        var products = await _products.GetAllWithCategoryAsync();
        var vm = new StockReportViewModel
        {
            Products = products,
            TotalProducts = products.Count(),
            LowStockCount = products.Count(p => p.IsLowStock),
            TotalStockValue = products.Sum(p => p.CurrentStock * p.PurchasePrice)
        };
        return View(vm);
    }

    public async Task<IActionResult> TopProducts(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddMonths(-1);
        var toDate = (to ?? DateTime.UtcNow.Date).AddDays(1);

        var topProducts = await _context.InvoiceItems
            .Include(ii => ii.Invoice)
            .Include(ii => ii.Product)
            .Where(ii => ii.Invoice!.InvoiceDate >= fromDate && ii.Invoice.InvoiceDate <= toDate
                && ii.Invoice.Status == Models.InvoiceStatus.Completed)
            .GroupBy(ii => new { ii.ProductId, ii.ProductName })
            .Select(g => new
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                TotalQuantity = g.Sum(ii => ii.Quantity),
                TotalRevenue = g.Sum(ii => ii.Total),
                TotalProfit = g.Sum(ii => (ii.UnitPrice - ii.PurchasePrice) * ii.Quantity)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(20)
            .ToListAsync();

        ViewBag.From = fromDate.ToString("yyyy-MM-dd");
        ViewBag.To = toDate.AddDays(-1).ToString("yyyy-MM-dd");
        return View(topProducts);
    }
}


