using Catalog.Domain.Errors;
using Catalog.Domain.ValueObjects;
using FluentAssertions;

namespace Catalog.Domain.Tests;

public sealed class CategoryTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var result = Category.Create("Electronics");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Electronics");
    }

    [Fact]
    public void Create_WithBlankName_ShouldReturnError()
    {
        var result = Category.Create("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryRequired);
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ShouldReturnError()
    {
        var result = Category.Create("   ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryRequired);
    }

    [Fact]
    public void Create_WithNameExceeding100Chars_ShouldReturnError()
    {
        var longName = new string('x', 101);
        var result = Category.Create(longName);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.CategoryTooLong);
    }

    [Fact]
    public void Create_WithNameExactly100Chars_ShouldSucceed()
    {
        var name = new string('x', 100);
        var result = Category.Create(name);

        result.IsSuccess.Should().BeTrue();
    }
}
