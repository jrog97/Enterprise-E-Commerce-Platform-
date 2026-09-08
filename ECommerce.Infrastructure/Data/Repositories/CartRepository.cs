using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ECommerceDbContext _dbContext;

    public CartRepository(
        ECommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShoppingCart?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShoppingCarts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(
                cart => cart.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        ShoppingCart cart,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ShoppingCarts.AddAsync(
            cart,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}