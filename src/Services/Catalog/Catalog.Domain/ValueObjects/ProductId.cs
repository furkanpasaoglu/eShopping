using Shared.BuildingBlocks.Domain.Primitives;
using Shared.BuildingBlocks.Guards;

namespace Catalog.Domain.ValueObjects;

public sealed class ProductId : ValueObject
{
    private ProductId(Guid value) => Value = value;

    public Guid Value { get; }

    public static ProductId New() => new(Guid.NewGuid());

    public static ProductId From(Guid value) =>
        new(Guard.AgainstDefault(value, nameof(value)));

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}
