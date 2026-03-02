using System.Net;
using System.Net.Http.Json;
using Catalog.Application.DTOs;
using FluentAssertions;
using Shared.BuildingBlocks.Pagination;

namespace Catalog.Integration.Tests;

public sealed class GetProductsEndpointTests(CatalogWebApplicationFactory factory)
    : IClassFixture<CatalogWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetProducts_WhenProductsExist_ShouldReturnPagedList()
    {
        var adminId = Guid.NewGuid().ToString();
        _client.DefaultRequestHeaders.Add("X-User-Id", adminId);
        _client.DefaultRequestHeaders.Add("X-User-Roles", "Admin");

        var created = await CreateProductAsync("Test Laptop", "Electronics", 999m, "USD", 10);
        created.StatusCode.Should().Be(HttpStatusCode.Created);

        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Remove("X-User-Roles");

        var response = await _client.GetAsync("/api/v1/catalog/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedList<ProductResponse>>();
        result.Should().NotBeNull();
        result!.TotalCount.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetProducts_WithCategoryFilter_ShouldReturnFilteredResults()
    {
        _client.DefaultRequestHeaders.Add("X-User-Id", Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add("X-User-Roles", "Admin");

        await CreateProductAsync("Category Filter Product", "UniqueTestCategory", 100m, "USD", 5);

        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Remove("X-User-Roles");

        var response = await _client.GetAsync("/api/v1/catalog/products?category=UniqueTestCategory");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedList<ProductResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(p => p.Category.Should().Be("UniqueTestCategory"));
    }

    [Fact]
    public async Task GetProducts_WithPagination_ShouldReturnCorrectPage()
    {
        _client.DefaultRequestHeaders.Add("X-User-Id", Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add("X-User-Roles", "Admin");

        await CreateProductAsync("Pagination Product A", "PaginationTest", 100m, "USD", 5);
        await CreateProductAsync("Pagination Product B", "PaginationTest", 200m, "USD", 5);
        await CreateProductAsync("Pagination Product C", "PaginationTest", 300m, "USD", 5);

        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Remove("X-User-Roles");

        var response = await _client.GetAsync(
            "/api/v1/catalog/products?category=PaginationTest&page=1&pageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedList<ProductResponse>>();
        result.Should().NotBeNull();
        result!.Items.Count.Should().Be(2);
        result.TotalCount.Should().Be(3);
    }

    private async Task<HttpResponseMessage> CreateProductAsync(
        string name, string category, decimal price, string currency, int stock)
    {
        var body = new
        {
            name,
            category,
            price,
            currency,
            stock,
            description = (string?)null,
            imageUrl = (string?)null
        };
        return await _client.PostAsJsonAsync("/api/v1/catalog/products", body);
    }
}
