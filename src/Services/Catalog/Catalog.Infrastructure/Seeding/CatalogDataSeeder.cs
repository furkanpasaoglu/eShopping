using Catalog.Application.ReadModels;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Elasticsearch;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Seeding;

internal sealed class CatalogDataSeeder(
    IServiceScopeFactory scopeFactory,
    ILogger<CatalogDataSeeder> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var esClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();

        var countResponse = await esClient.CountAsync<ProductReadModel>(
            CatalogIndexMapping.IndexName, cancellationToken);

        if (countResponse.IsValidResponse && countResponse.Count > 0)
        {
            logger.LogInformation("Catalog already seeded with {Count} products. Skipping.", countResponse.Count);
            return;
        }

        logger.LogInformation("Seeding catalog with sample data...");

        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var seeds = new[]
        {
            ("Laptop Pro 15", "Electronics", 1299.99m, "USD", 50, "High-performance laptop with 15\" display", null as string),
            ("Wireless Mouse", "Electronics", 29.99m, "USD", 200, "Ergonomic wireless mouse with long battery life", null),
            ("Mechanical Keyboard", "Electronics", 89.99m, "USD", 150, "Tactile mechanical keyboard with RGB backlight", null),
            ("USB-C Hub", "Electronics", 49.99m, "USD", 300, "7-in-1 USB-C hub with HDMI and USB 3.0 ports", null),
            ("Running Shoes", "Footwear", 119.99m, "USD", 80, "Lightweight running shoes with cushioned sole", null),
            ("Yoga Mat", "Sports", 35.99m, "USD", 120, "Non-slip 6mm yoga mat with carrying strap", null),
            ("Water Bottle", "Sports", 24.99m, "USD", 500, "1L stainless steel insulated water bottle", null),
            ("Backpack 30L", "Accessories", 79.99m, "USD", 60, "Durable 30L backpack with laptop compartment", null),
            ("Desk Lamp", "Home", 39.99m, "USD", 90, "LED desk lamp with adjustable brightness and color temp", null),
            ("Notebook Set", "Stationery", 14.99m, "USD", 400, "Set of 3 ruled notebooks, A5 size", null)
        };

        foreach (var (name, category, price, currency, stock, description, imageUrl) in seeds)
        {
            var result = Product.Create(name, price, currency, category, stock, description, imageUrl);
            if (result.IsFailure)
            {
                logger.LogWarning("Failed to create seed product {Name}: {Error}", name, result.Error);
                continue;
            }

            var product = result.Value;

            var doc = new ProductDocument
            {
                Id = product.Id.Value,
                Name = product.Name.Value,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                Category = product.Category.Name,
                Stock = product.Stock.Value,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                IsDeleted = product.IsDeleted,
                DeletedAt = product.DeletedAt
            };

            var readModel = new ProductReadModel
            {
                Id = product.Id.Value,
                Name = product.Name.Value,
                Category = product.Category.Name,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                Stock = product.Stock.Value,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                IsDeleted = false
            };

            await dbContext.Products.InsertOneAsync(doc, cancellationToken: cancellationToken);
            await esClient.IndexAsync(
                readModel,
                r => r.Index(CatalogIndexMapping.IndexName).Id(readModel.Id.ToString()),
                cancellationToken);
        }

        logger.LogInformation("Catalog seeded with {Count} products.", seeds.Length);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
