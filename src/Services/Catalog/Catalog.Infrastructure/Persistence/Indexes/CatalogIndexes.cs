using Catalog.Application.ReadModels;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Persistence.Indexes;

internal static class CatalogIndexes
{
    public static async Task EnsureIndexesAsync(CatalogDbContext dbContext)
    {
        var collection = dbContext.ProductViews;

        var compoundIndex = Builders<ProductReadModel>.IndexKeys
            .Ascending(m => m.Category)
            .Ascending(m => m.Price);

        var textIndex = Builders<ProductReadModel>.IndexKeys
            .Text(m => m.Name);

        var deletedIndex = Builders<ProductReadModel>.IndexKeys
            .Ascending(m => m.IsDeleted);

        await collection.Indexes.CreateManyAsync(
        [
            new CreateIndexModel<ProductReadModel>(compoundIndex),
            new CreateIndexModel<ProductReadModel>(textIndex),
            new CreateIndexModel<ProductReadModel>(deletedIndex)
        ]);
    }
}
