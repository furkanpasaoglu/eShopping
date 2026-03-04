using MongoDB.Driver;

namespace Catalog.Infrastructure.Persistence;

internal sealed class CatalogDbContext(IMongoDatabase database)
{
    public IMongoCollection<ProductDocument> Products =>
        database.GetCollection<ProductDocument>(MongoCollectionNames.Products);
}
