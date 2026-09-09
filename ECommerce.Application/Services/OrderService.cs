using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderDto> CreateOrderAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart == null || !cart.Items.Any())
        {
            throw new InvalidOperationException(
                "Cannot create an order from an empty cart.");
        }

        decimal subtotal = 0;

        var checkoutItems = new List<(
            Guid ProductId,
            string ProductName,
            string SKU,
            decimal UnitPrice,
            int Quantity)>();

        foreach (var cartItem in cart.Items)
        {
            var product = await _productRepository.GetByIdForUpdateAsync(
                cartItem.ProductId,
                cancellationToken);

            if (product == null)
            {
                throw new InvalidOperationException(
                    $"Product {cartItem.ProductId} no longer exists.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException(
                    $"Product '{product.Name}' is no longer available.");
            }

            if (!product.RemoveStock(cartItem.Quantity))
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product '{product.Name}'.");
            }

            checkoutItems.Add((
                product.Id,
                product.Name,
                product.SKU,
                product.Price,
                cartItem.Quantity));

            subtotal += product.Price * cartItem.Quantity;
        }

        subtotal = Math.Round(
            subtotal,
            2,
            MidpointRounding.AwayFromZero);

        const decimal taxRate = 0.07m;

        var tax = Math.Round(
            subtotal * taxRate,
            2,
            MidpointRounding.AwayFromZero);

        var order = new Order(
            userId,
            subtotal,
            tax);

        foreach (var item in checkoutItems)
        {
            order.Items.Add(
                new OrderItem(
                    order.Id,
                    item.ProductId,
                    item.ProductName,
                    item.SKU,
                    item.UnitPrice,
                    item.Quantity));
        }

        order.Confirm();

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        await _cartRepository.DeleteAsync(cart);

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
