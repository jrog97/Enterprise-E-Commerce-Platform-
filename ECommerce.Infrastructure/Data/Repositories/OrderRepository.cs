using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ECommerceDbContext _dbContext;

    public OrderRepository(
        ECommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Order>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .FirstOrDefaultAsync(
                order => order.Id == orderId,
                cancellationToken);
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(
            order,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<List<Order>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
    return await _dbContext.Orders
        .AsNoTracking()
        .Include(order => order.Items)
        .OrderByDescending(order => order.CreatedAt)
        .ToListAsync(cancellationToken);
    }
}