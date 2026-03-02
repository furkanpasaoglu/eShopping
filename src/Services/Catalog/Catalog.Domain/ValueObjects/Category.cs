using Catalog.Domain.Errors;
using Shared.BuildingBlocks.Domain.Primitives;
using Shared.BuildingBlocks.Results;

namespace Catalog.Domain.ValueObjects;

public sealed class Category : ValueObject
{
    private const int MaxLength = 100;

    private Category(string name) => Name = name;

    public string Name { get; }

    public static Result<Category> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProductErrors.CategoryRequired;

        if (name.Length > MaxLength)
            return ProductErrors.CategoryTooLong;

        return new Category(name);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Name;
    }
}
