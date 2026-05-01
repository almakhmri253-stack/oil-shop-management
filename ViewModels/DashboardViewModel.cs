using OilShopManagement.Models;

namespace OilShopManagement.ViewModels;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalInvoicesToday { get; set; }
    public int LowStockCount { get; set; }

    public decimal SalesToday { get; set; }
    public decimal SalesThisMonth { get; set; }
    public decimal ProfitToday { get; set; }
    public decimal ProfitThisMonth { get; set; }

    public IEnumerable<Invoice> RecentInvoices { get; set; } = [];
    public IEnumerable<Product> LowStockProducts { get; set; } = [];
    public IEnumerable<StockTransaction> RecentStockMovements { get; set; } = [];

    public List<string> ChartLabels { get; set; } = [];
    public List<decimal> ChartSalesData { get; set; } = [];
    public List<decimal> ChartProfitData { get; set; } = [];
}


