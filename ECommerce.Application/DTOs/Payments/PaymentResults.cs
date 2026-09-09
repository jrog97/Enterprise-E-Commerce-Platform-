namespace ECommerce.Application.DTOs.Payments;

public class PaymentResult
{
    public bool Successful { get; init; }
    public string TransactionId { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }

    public static PaymentResult Success(string transactionId)
    {
        return new PaymentResult
        {
            Successful = true,
            TransactionId = transactionId
        };
    }

    public static PaymentResult Failure(string errorMessage)
    {
        return new PaymentResult
        {
            Successful = false,
            ErrorMessage = errorMessage
        };
    }
}