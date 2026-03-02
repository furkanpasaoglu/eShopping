using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Abstractions;

public interface IProductWriteRepository
{
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
}
