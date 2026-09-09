using ECommerce.Application.DTOs.Payments;
using ECommerce.Application.Interfaces;

namespace ECommerce.Infrastructure.Payments;

public class MockPaymentProcessor : IPaymentProcessor
{
    public Task<PaymentResult> ProcessPaymentAsync(
        Guid orderId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            return Task.FromResult(
                PaymentResult.Failure(
                    "Payment amount must be greater than zero."));
        }

        var transactionId =
            $"MOCK-{Guid.NewGuid():N}";

        return Task.FromResult(
            PaymentResult.Success(transactionId));
    }
}