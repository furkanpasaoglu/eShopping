using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Elasticsearch;
using Catalog.Infrastructure.Persistence.Indexes;
using Catalog.Infrastructure.Persistence.Repositories;
using Catalog.Infrastructure.Seeding;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("catalog-db")
            ?? throw new InvalidOperationException(
                "MongoDB connection string 'catalog-db' is not configured.");

        BsonMappingConfig.Register();

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("catalog"));
        services.AddSingleton<CatalogDbContext>();

        var esConnectionString = configuration.GetConnectionString("elasticsearch")
            ?? throw new InvalidOperationException(
                "Elasticsearch connection string 'elasticsearch' is not configured.");

        services.AddSingleton<ElasticsearchClient>(_ =>
            new ElasticsearchClient(new Uri(esConnectionString)));

        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        services.AddScoped<IProductReadRepository, ProductElasticsearchReadRepository>();

        services.AddHostedService<CatalogElasticsearchInitializer>();
        services.AddHostedService<CatalogDataSeeder>();

        return services;
    }

    public static async Task EnsureIndexesAsync(IServiceProvider services)
    {
        var dbContext = services.GetRequiredService<CatalogDbContext>();
        await CatalogIndexes.EnsureIndexesAsync(dbContext);
    }
}
