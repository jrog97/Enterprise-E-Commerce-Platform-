using ECommerce.Application.DTOs.Orders;

namespace ECommerce.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderDto>> GetOrdersAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<OrderDto?> GetOrderByIdAsync(
        Guid userId,
        Guid orderId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(
    CancellationToken cancellationToken = default);

    Task<OrderDto> ConfirmOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<OrderDto> StartProcessingAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<OrderDto> ShipOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<OrderDto> DeliverOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<OrderDto> CancelOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}