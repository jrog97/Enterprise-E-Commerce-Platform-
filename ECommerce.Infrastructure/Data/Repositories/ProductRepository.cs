using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ECommerceDbContext _dbContext;

    public ProductRepository(ECommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AnyAsync(
                product => product.SKU == sku,
                cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(
            product,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
    public async Task<bool> CategoryExistsAsync(
    Guid categoryId,
    CancellationToken cancellationToken = default)
{
    return await _dbContext.Categories
        .AnyAsync(
            category => category.Id == categoryId,
            cancellationToken);
}

public async Task<Product?> GetByIdForUpdateAsync(
    Guid id,
    CancellationToken cancellationToken = default)
{
    return await _dbContext.Products
        .FirstOrDefaultAsync(
            product => product.Id == id,
            cancellationToken);
}

}