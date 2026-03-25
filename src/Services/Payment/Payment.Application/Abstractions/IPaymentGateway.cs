namespace Payment.Application.Abstractions;

public interface IPaymentGateway
{
    Task<PaymentGatewayResult> ProcessAsync(Guid orderId, decimal amount, string currency, CancellationToken ct = default);
}

public sealed record PaymentGatewayResult(bool Success, string TransactionId, string? FailureReason = null);
