using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;
using OilShopManagement.Repositories;
using OilShopManagement.ViewModels;

namespace OilShopManagement.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IProductRepository _products;
    private readonly IInvoiceRepository _invoices;
    private readonly IStockRepository _stock;
    private readonly ApplicationDbContext _context;

    public DashboardController(IProductRepository products, IInvoiceRepository invoices,
        IStockRepository stock, ApplicationDbContext context)
    {
        _products = products;
        _invoices = invoices;
        _stock = stock;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var tomorrow = today.AddDays(1);

        var vm = new DashboardViewModel
        {
            TotalProducts = await _products.CountAsync(p => p.IsActive),
            TotalCustomers = await _context.Customers.CountAsync(c => c.IsActive),
            TotalInvoicesToday = await _invoices.CountAsync(i => i.InvoiceDate >= today && i.InvoiceDate < tomorrow && i.Status == InvoiceStatus.Completed),
            LowStockCount = (await _products.GetLowStockProductsAsync()).Count(),
            SalesToday = await _invoices.GetTotalSalesAsync(today, tomorrow),
            SalesThisMonth = await _invoices.GetTotalSalesAsync(monthStart, tomorrow),
            ProfitToday = await _invoices.GetTotalProfitAsync(today, tomorrow),
            ProfitThisMonth = await _invoices.GetTotalProfitAsync(monthStart, tomorrow),
            RecentInvoices = await _invoices.GetByDateRangeAsync(today.AddDays(-7), tomorrow),
            LowStockProducts = await _products.GetLowStockProductsAsync(),
            RecentStockMovements = await _stock.GetRecentAsync(10)
        };

        // Chart data - last 7 days
        for (int i = 6; i >= 0; i--)
        {
            var day = today.AddDays(-i);
            var nextDay = day.AddDays(1);
            vm.ChartLabels.Add(day.ToString("dd/MM"));
            vm.ChartSalesData.Add(await _invoices.GetTotalSalesAsync(day, nextDay));
            vm.ChartProfitData.Add(await _invoices.GetTotalProfitAsync(day, nextDay));
        }

        return View(vm);
    }
}


