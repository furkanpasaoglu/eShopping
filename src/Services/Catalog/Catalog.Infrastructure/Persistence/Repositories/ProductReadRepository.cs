using Catalog.Application.Abstractions;
using Catalog.Application.ReadModels;
using MongoDB.Driver;
using Shared.BuildingBlocks.Pagination;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductReadRepository(CatalogDbContext dbContext) : IProductReadRepository
{
    public async Task<ProductReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<ProductReadModel>.Filter.And(
            Builders<ProductReadModel>.Filter.Eq(m => m.Id, id),
            Builders<ProductReadModel>.Filter.Eq(m => m.IsDeleted, false));

        return await dbContext.ProductViews.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<(IReadOnlyList<ProductReadModel> Items, int TotalCount)> GetPagedAsync(
        string? category,
        string? name,
        decimal? minPrice,
        decimal? maxPrice,
        PaginationParams pagination,
        CancellationToken ct = default)
    {
        var filterBuilder = Builders<ProductReadModel>.Filter;
        var filter = filterBuilder.Eq(m => m.IsDeleted, false);

        if (!string.IsNullOrWhiteSpace(category))
            filter &= filterBuilder.Eq(m => m.Category, category);

        if (!string.IsNullOrWhiteSpace(name))
            filter &= filterBuilder.Regex(m => m.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));

        if (minPrice.HasValue)
            filter &= filterBuilder.Gte(m => m.Price, minPrice.Value);

        if (maxPrice.HasValue)
            filter &= filterBuilder.Lte(m => m.Price, maxPrice.Value);

        var countTask = dbContext.ProductViews.CountDocumentsAsync(filter, cancellationToken: ct);
        var itemsTask = dbContext.ProductViews
            .Find(filter)
            .Skip(pagination.Skip)
            .Limit(pagination.PageSize)
            .ToListAsync(ct);

        await Task.WhenAll(countTask, itemsTask);

        return (itemsTask.Result.AsReadOnly(), (int)countTask.Result);
    }

    public async Task UpsertAsync(ProductReadModel model, CancellationToken ct = default)
    {
        var filter = Builders<ProductReadModel>.Filter.Eq(m => m.Id, model.Id);
        var options = new ReplaceOptions { IsUpsert = true };
        await dbContext.ProductViews.ReplaceOneAsync(filter, model, options, ct);
    }

    public async Task MarkDeletedAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<ProductReadModel>.Filter.Eq(m => m.Id, id);
        var update = Builders<ProductReadModel>.Update
            .Set(m => m.IsDeleted, true)
            .Set(m => m.UpdatedAt, DateTime.UtcNow);
        await dbContext.ProductViews.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}
