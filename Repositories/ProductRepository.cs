using Microsoft.EntityFrameworkCore;
using OilShopManagement.Data;
using OilShopManagement.Models;

namespace OilShopManagement.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync() =>
        await _dbSet.Include(p => p.Category).Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();

    public async Task<Product?> GetWithCategoryAsync(int id) =>
        await _dbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync() =>
        await _dbSet.Include(p => p.Category)
            .Where(p => p.IsActive && p.CurrentStock <= p.MinStockLevel)
            .ToListAsync();

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm) =>
        await _dbSet.Include(p => p.Category)
            .Where(p => p.IsActive && (p.Name.Contains(searchTerm) || (p.SKU != null && p.SKU.Contains(searchTerm))))
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
        await _dbSet.Include(p => p.Category)
            .Where(p => p.IsActive && p.CategoryId == categoryId)
            .ToListAsync();
}


