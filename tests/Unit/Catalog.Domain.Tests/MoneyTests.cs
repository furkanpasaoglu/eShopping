using Catalog.Domain.Errors;
using Catalog.Domain.ValueObjects;
using FluentAssertions;

namespace Catalog.Domain.Tests;

public sealed class MoneyTests
{
    [Fact]
    public void Create_WithValidArgs_ShouldSucceed()
    {
        var result = Money.Create(100m, "USD");

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(100m);
        result.Value.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldSucceed()
    {
        var result = Money.Create(0m, "USD");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldReturnError()
    {
        var result = Money.Create(-0.01m, "USD");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NegativePrice);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDD")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidCurrency_ShouldReturnError(string currency)
    {
        var result = Money.Create(100m, currency);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCurrency);
    }

    [Fact]
    public void TwoMoneyObjects_WithSameValues_ShouldBeEqual()
    {
        var a = Money.Create(100m, "USD").Value;
        var b = Money.Create(100m, "USD").Value;

        a.Should().Be(b);
    }

    [Fact]
    public void TwoMoneyObjects_WithDifferentAmounts_ShouldNotBeEqual()
    {
        var a = Money.Create(100m, "USD").Value;
        var b = Money.Create(200m, "USD").Value;

        a.Should().NotBe(b);
    }
}
