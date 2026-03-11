using Basket.Application.DTOs;

namespace Basket.Application.Abstractions;

public interface ICatalogGrpcClient
{
    Task<ProductSnapshot?> GetProductSnapshotAsync(Guid productId, CancellationToken ct = default);
}
