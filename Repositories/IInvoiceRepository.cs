using OilShopManagement.Models;

namespace OilShopManagement.Repositories;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetAllWithDetailsAsync();
    Task<Invoice?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId);
    Task<string> GenerateInvoiceNumberAsync();
    Task<decimal> GetTotalSalesAsync(DateTime from, DateTime to);
    Task<decimal> GetTotalProfitAsync(DateTime from, DateTime to);
}


