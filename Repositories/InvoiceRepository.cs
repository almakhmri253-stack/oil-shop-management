using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;

namespace OilShopManagement.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Invoice>> GetAllWithDetailsAsync() =>
        await _dbSet.Include(i => i.Customer).Include(i => i.Items).ThenInclude(it => it.Product)
            .OrderByDescending(i => i.InvoiceDate).ToListAsync();

    public async Task<Invoice?> GetWithDetailsAsync(int id) =>
        await _dbSet.Include(i => i.Customer).Include(i => i.Items).ThenInclude(it => it.Product)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to) =>
        await _dbSet.Include(i => i.Customer).Include(i => i.Items)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.Status == InvoiceStatus.Completed)
            .OrderByDescending(i => i.InvoiceDate).ToListAsync();

    public async Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId) =>
        await _dbSet.Include(i => i.Items)
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.InvoiceDate).ToListAsync();

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var today = DateTime.UtcNow;
        var prefix = $"INV-{today:yyyyMM}-";
        var lastInvoice = await _dbSet
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        int seq = 1;
        if (lastInvoice != null)
        {
            var parts = lastInvoice.InvoiceNumber.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[^1], out int lastSeq))
                seq = lastSeq + 1;
        }
        return $"{prefix}{seq:D4}";
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime from, DateTime to) =>
        await _dbSet.Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.Status == InvoiceStatus.Completed)
            .SumAsync(i => i.TotalAmount);

    public async Task<decimal> GetTotalProfitAsync(DateTime from, DateTime to) =>
        await _context.InvoiceItems
            .Include(ii => ii.Invoice)
            .Where(ii => ii.Invoice!.InvoiceDate >= from && ii.Invoice.InvoiceDate <= to && ii.Invoice.Status == InvoiceStatus.Completed)
            .SumAsync(ii => (ii.UnitPrice - ii.PurchasePrice) * ii.Quantity);
}


