using Catalog.Domain.Errors;
using Shared.BuildingBlocks.Domain.Primitives;
using Shared.BuildingBlocks.Results;

namespace Catalog.Domain.ValueObjects;

public sealed class ProductName : ValueObject
{
    private const int MaxLength = 200;

    private ProductName(string value) => Value = value;

    public string Value { get; }

    public static Result<ProductName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ProductErrors.NameRequired;

        if (value.Length > MaxLength)
            return ProductErrors.NameTooLong;

        return new ProductName(value);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}
