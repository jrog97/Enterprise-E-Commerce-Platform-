using ECommerce.Application.DTOs.Payments;

namespace ECommerce.Application.Interfaces;

public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessPaymentAsync(
        Guid orderId,
        decimal amount,
        CancellationToken cancellationToken = default);
}