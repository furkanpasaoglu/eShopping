using Catalog.Domain.Errors;
using Catalog.Domain.ValueObjects;
using FluentAssertions;

namespace Catalog.Domain.Tests;

public sealed class StockQuantityTests
{
    [Fact]
    public void Create_WithZero_ShouldSucceed()
    {
        var result = StockQuantity.Create(0);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(0);
    }

    [Fact]
    public void Create_WithPositiveValue_ShouldSucceed()
    {
        var result = StockQuantity.Create(100);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(100);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnError()
    {
        var result = StockQuantity.Create(-1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NegativeStock);
    }
}
