using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;

namespace OilShopManagement.Repositories;

public class StockRepository : Repository<StockTransaction>, IStockRepository
{
    public StockRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<StockTransaction>> GetByProductAsync(int productId) =>
        await _dbSet.Include(st => st.Product)
            .Where(st => st.ProductId == productId)
            .OrderByDescending(st => st.CreatedAt).ToListAsync();

    public async Task<IEnumerable<StockTransaction>> GetRecentAsync(int count = 20) =>
        await _dbSet.Include(st => st.Product)
            .OrderByDescending(st => st.CreatedAt).Take(count).ToListAsync();
}
