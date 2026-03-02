using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Indexes;
using Catalog.Infrastructure.Persistence.Repositories;
using Catalog.Infrastructure.Seeding;
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
        var connectionString = configuration.GetConnectionString("catalog-db")
            ?? throw new InvalidOperationException(
                "MongoDB connection string 'catalog-db' is not configured.");

        BsonMappingConfig.Register();

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("catalog"));
        services.AddSingleton<CatalogDbContext>();

        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        services.AddScoped<IProductReadRepository, ProductReadRepository>();

        services.AddHostedService<CatalogDataSeeder>();

        return services;
    }

    public static async Task EnsureIndexesAsync(IServiceProvider services)
    {
        var dbContext = services.GetRequiredService<CatalogDbContext>();
        await CatalogIndexes.EnsureIndexesAsync(dbContext);
    }
}
