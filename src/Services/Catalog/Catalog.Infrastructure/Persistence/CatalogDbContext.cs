using Catalog.Application.ReadModels;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Persistence;

internal sealed class CatalogDbContext(IMongoDatabase database)
{
    public IMongoCollection<ProductDocument> Products =>
        database.GetCollection<ProductDocument>(MongoCollectionNames.Products);

    public IMongoCollection<ProductReadModel> ProductViews =>
        database.GetCollection<ProductReadModel>(MongoCollectionNames.ProductViews);
}
