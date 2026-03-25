using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.Application.Abstractions;

namespace Payment.Infrastructure.Gateway;

internal sealed class FakePaymentGateway(
    IOptions<FakePaymentOptions> options,
    ILogger<FakePaymentGateway> logger) : IPaymentGateway
{
    public async Task<PaymentGatewayResult> ProcessAsync(
        Guid orderId, decimal amount, string currency, CancellationToken ct = default)
    {
        await Task.Delay(options.Value.ProcessingDelayMs, ct);

        var transactionId = $"FAKE-{Guid.NewGuid():N}";

        if (Random.Shared.NextDouble() < options.Value.FailureRate)
        {
            logger.LogWarning("Fake payment DECLINED for order {OrderId}, amount {Amount} {Currency}",
                orderId, amount, currency);
            return new PaymentGatewayResult(false, transactionId, "Insufficient funds (simulated)");
        }

        logger.LogInformation("Fake payment APPROVED for order {OrderId}, amount {Amount} {Currency}",
            orderId, amount, currency);
        return new PaymentGatewayResult(true, transactionId);
    }
}
