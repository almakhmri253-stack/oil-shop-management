using OilShopManagement.Models;

namespace OilShopManagement.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<Product?> GetWithCategoryAsync(int id);
    Task<IEnumerable<Product>> GetLowStockProductsAsync();
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
}

