namespace Payment.Infrastructure.Gateway;

public sealed class FakePaymentOptions
{
    public double FailureRate { get; set; } = 0.0;
    public int ProcessingDelayMs { get; set; } = 100;
}
