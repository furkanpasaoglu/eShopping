using System.Net;
using System.Net.Http.Json;
using Catalog.Application.DTOs;
using FluentAssertions;

namespace Catalog.Integration.Tests;

public sealed class CreateProductEndpointTests(CatalogWebApplicationFactory factory)
    : IClassFixture<CatalogWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateProduct_WithNoToken_ShouldReturn401()
    {
        var body = ValidProductBody();
        var response = await _client.PostAsJsonAsync("/api/v1/catalog/products", body);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProduct_WithCustomerRole_ShouldReturn403()
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-User-Id", Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add("X-User-Roles", "Customer");

        var response = await _client.PostAsJsonAsync("/api/v1/catalog/products", ValidProductBody());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateProduct_WithAdminRole_ValidBody_ShouldReturn201WithLocationHeader()
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-User-Id", Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add("X-User-Roles", "Admin");

        var response = await _client.PostAsJsonAsync("/api/v1/catalog/products", ValidProductBody());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/v1/catalog/products/");
    }

    [Fact]
    public async Task CreateProduct_WithBlankName_ShouldReturn422WithProblemDetails()
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-User-Id", Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add("X-User-Roles", "Admin");

        var body = new { name = "", category = "Electronics", price = 100m, currency = "USD", stock = 10 };
        var response = await _client.PostAsJsonAsync("/api/v1/catalog/products", body);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateProduct_ThenGetById_ShouldReturnCreatedProduct()
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-User-Id", Guid.NewGuid().ToString());
        _client.DefaultRequestHeaders.Add("X-User-Roles", "Admin");

        var createResponse = await _client.PostAsJsonAsync("/api/v1/catalog/products", ValidProductBody());
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = await createResponse.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();

        _client.DefaultRequestHeaders.Clear();
        var getResponse = await _client.GetAsync($"/api/v1/catalog/products/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();
        product.Should().NotBeNull();
        product!.Name.Should().Be("Integration Test Product");
    }

    private static object ValidProductBody() => new
    {
        name = "Integration Test Product",
        category = "Electronics",
        price = 999.99m,
        currency = "USD",
        stock = 25,
        description = "A product for integration testing",
        imageUrl = (string?)null
    };
}
