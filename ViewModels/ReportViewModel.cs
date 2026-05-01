using OilShopManagement.Models;

namespace OilShopManagement.ViewModels;

public class SalesReportViewModel
{
    public DateTime FromDate { get; set; } = DateTime.UtcNow.Date.AddDays(-30);
    public DateTime ToDate { get; set; } = DateTime.UtcNow.Date;
    public IEnumerable<Invoice> Invoices { get; set; } = [];
    public decimal TotalSales { get; set; }
    public decimal TotalProfit { get; set; }
    public int TotalInvoices { get; set; }
}

public class StockReportViewModel
{
    public IEnumerable<Product> Products { get; set; } = [];
    public int TotalProducts { get; set; }
    public int LowStockCount { get; set; }
    public decimal TotalStockValue { get; set; }
}


