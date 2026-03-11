using Basket.Application.Abstractions;
using Basket.Application.DTOs;
using Grpc.Core;
using Shared.Grpc.Catalog;

namespace Basket.Infrastructure.Grpc;

internal sealed class CatalogGrpcClient(CatalogGrpcService.CatalogGrpcServiceClient client)
    : ICatalogGrpcClient
{
    public async Task<ProductSnapshot?> GetProductSnapshotAsync(Guid productId, CancellationToken ct = default)
    {
        GetProductByIdResponse response;

        try
        {
            response = await client.GetProductByIdAsync(
                new GetProductByIdRequest { ProductId = productId.ToString() },
                cancellationToken: ct);
        }
        catch (RpcException ex) when (
            ex.StatusCode is StatusCode.Unavailable
                or StatusCode.DeadlineExceeded
                or StatusCode.Internal)
        {
            throw new CatalogServiceUnavailableException(
                "Catalog gRPC service is unreachable.", ex);
        }

        if (!response.Found || response.Product is null)
            return null;

        var dto = response.Product;
        return new ProductSnapshot(
            Guid.Parse(dto.Id),
            dto.Name,
            (decimal)dto.Price,
            dto.Currency,
            dto.Stock);
    }
}
