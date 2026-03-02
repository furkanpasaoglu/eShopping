using Catalog.Domain.Errors;
using Shared.BuildingBlocks.Domain.Primitives;
using Shared.BuildingBlocks.Results;

namespace Catalog.Domain.ValueObjects;

public sealed class StockQuantity : ValueObject
{
    private StockQuantity(int value) => Value = value;

    public int Value { get; }

    public static Result<StockQuantity> Create(int value)
    {
        if (value < 0)
            return ProductErrors.NegativeStock;

        return new StockQuantity(value);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}
