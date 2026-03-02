using Catalog.Domain.Errors;
using Shared.BuildingBlocks.Domain.Primitives;
using Shared.BuildingBlocks.Results;

namespace Catalog.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Result<Money> Create(decimal amount, string? currency)
    {
        if (amount < 0)
            return ProductErrors.NegativePrice;

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            return ProductErrors.InvalidCurrency;

        return new Money(amount, currency.ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
