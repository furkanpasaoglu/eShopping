using Catalog.Domain.Entities;
using Catalog.Domain.Errors;
using Catalog.Domain.Events;
using FluentAssertions;

namespace Catalog.Domain.Tests;

public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidArgs_ShouldSucceed_AndRaiseProductCreatedDomainEvent()
    {
        var result = Product.Create("Laptop", 999.99m, "USD", "Electronics", 10, null, null);

        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductCreatedDomainEvent>();
    }

    [Fact]
    public void Create_WithBlankName_ShouldReturnValidationError()
    {
        var result = Product.Create("", 999.99m, "USD", "Electronics", 10, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NameRequired);
    }

    [Fact]
    public void Create_WithNameExceeding200Chars_ShouldReturnNameTooLongError()
    {
        var longName = new string('a', 201);
        var result = Product.Create(longName, 999.99m, "USD", "Electronics", 10, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NameTooLong);
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldReturnValidationError()
    {
        var result = Product.Create("Laptop", -1m, "USD", "Electronics", 10, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NegativePrice);
    }

    [Fact]
    public void Create_WithInvalidCurrency_ShouldReturnValidationError()
    {
        var result = Product.Create("Laptop", 100m, "US", "Electronics", 10, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCurrency);
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldReturnValidationError()
    {
        var result = Product.Create("Laptop", 100m, "USD", "Electronics", -1, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NegativeStock);
    }

    [Fact]
    public void AdjustStock_WhenDeltaExceedsStock_ShouldReturnInsufficientStockError()
    {
        var product = Product.Create("Laptop", 999m, "USD", "Electronics", 5, null, null).Value;
        product.ClearDomainEvents();

        var result = product.AdjustStock(-10);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InsufficientStock);
    }

    [Fact]
    public void AdjustStock_WithValidDelta_ShouldUpdateStockAndRaiseDomainEvent()
    {
        var product = Product.Create("Laptop", 999m, "USD", "Electronics", 10, null, null).Value;
        product.ClearDomainEvents();

        var result = product.AdjustStock(-3);

        result.IsSuccess.Should().BeTrue();
        product.Stock.Value.Should().Be(7);
        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<StockAdjustedDomainEvent>();
    }

    [Fact]
    public void UpdateDetails_WhenPriceChanges_ShouldRaisePriceChangedEvent()
    {
        var product = Product.Create("Laptop", 999m, "USD", "Electronics", 10, null, null).Value;
        product.ClearDomainEvents();

        var result = product.UpdateDetails("Laptop Pro", 1299m, "USD", "Electronics", null, null);

        result.IsSuccess.Should().BeTrue();
        product.DomainEvents.Should().Contain(e => e is ProductPriceChangedDomainEvent);
    }

    [Fact]
    public void UpdateDetails_WhenPriceUnchanged_ShouldNotRaisePriceChangedEvent()
    {
        var product = Product.Create("Laptop", 999m, "USD", "Electronics", 10, null, null).Value;
        product.ClearDomainEvents();

        var result = product.UpdateDetails("Laptop Pro", 999m, "USD", "Electronics", null, null);

        result.IsSuccess.Should().BeTrue();
        product.DomainEvents.Should().NotContain(e => e is ProductPriceChangedDomainEvent);
    }

    [Fact]
    public void Delete_WhenNotDeleted_ShouldSucceedAndMarkDeleted()
    {
        var product = Product.Create("Laptop", 999m, "USD", "Electronics", 10, null, null).Value;
        product.ClearDomainEvents();

        var result = product.Delete();

        result.IsSuccess.Should().BeTrue();
        product.IsDeleted.Should().BeTrue();
        product.DeletedAt.Should().NotBeNull();
        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ShouldReturnAlreadyDeletedError()
    {
        var product = Product.Create("Laptop", 999m, "USD", "Electronics", 10, null, null).Value;
        product.Delete();
        product.ClearDomainEvents();

        var result = product.Delete();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.AlreadyDeleted);
    }
}
