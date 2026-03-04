using MongoDB.Driver;

namespace Catalog.Infrastructure.Persistence.Indexes;

internal static class CatalogIndexes
{
    public static async Task EnsureIndexesAsync(CatalogDbContext dbContext)
    {
        var deletedIndex = Builders<ProductDocument>.IndexKeys
            .Ascending(d => d.IsDeleted);

        await dbContext.Products.Indexes.CreateOneAsync(
            new CreateIndexModel<ProductDocument>(deletedIndex));
    }
}
