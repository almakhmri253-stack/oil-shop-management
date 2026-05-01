using OilShopManagement.Models;

namespace OilShopManagement.Repositories;

public interface IStockRepository : IRepository<StockTransaction>
{
    Task<IEnumerable<StockTransaction>> GetByProductAsync(int productId);
    Task<IEnumerable<StockTransaction>> GetRecentAsync(int count = 20);
}
