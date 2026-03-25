using Shared.BuildingBlocks.Domain.Primitives;

namespace Payment.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    private Money() { }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    public static Money Create(decimal amount, string currency) =>
        new(amount, currency.ToUpperInvariant());

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
