using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
    }

    public async Task<OrderDto> CreateOrderAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot create an order from an empty cart.");
        }

        var subtotal = cart.Items.Sum(
            item => item.Product.Price * item.Quantity);

        var tax = subtotal * 0.07m;

        var order = new Order(
            userId,
            subtotal,
            tax);

        foreach (var cartItem in cart.Items)
        {
            var orderItem = new OrderItem(
                order.Id,
                cartItem.ProductId,
                cartItem.Product.Name,
                cartItem.Product.SKU,
                cartItem.Product.Price,
                cartItem.Quantity);

            order.Items.Add(orderItem);
        }

        order.Confirm();

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        await _cartRepository.DeleteAsync(
            cart,
            cancellationToken);

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        return orders
            .Select(MapToDto)
            .ToList();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(
        Guid userId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null || order.UserId != userId)
        {
            return null;
        }

        return MapToDto(order);
    }

    private static OrderDto MapToDto(
        Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status.ToString(),
            Subtotal = order.Subtotal,
            Tax = order.Tax,
            Total = order.Total,
            CreatedAt = order.CreatedAt,

            Items = order.Items
                .Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    SKU = item.SKU,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                })
                .ToList()
        };
    }
}