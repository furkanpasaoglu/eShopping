using Catalog.Application.Abstractions;
using Grpc.Core;
using Shared.Grpc.Catalog;

namespace Catalog.API.Grpc;

internal sealed class CatalogGrpcService(IProductReadRepository readRepository)
    : Shared.Grpc.Catalog.CatalogGrpcService.CatalogGrpcServiceBase
{
    public override async Task<GetProductByIdResponse> GetProductById(
        GetProductByIdRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "product_id must be a valid UUID."));

        var model = await readRepository.GetByIdAsync(id, context.CancellationToken);

        if (model is null || model.IsDeleted)
            return new GetProductByIdResponse { Found = false };

        return new GetProductByIdResponse
        {
            Found = true,
            Product = new ProductDto
            {
                Id = model.Id.ToString(),
                Name = model.Name,
                Price = (double)model.Price,
                Currency = model.Currency,
                Category = model.Category,
                Stock = model.Stock,
                Description = model.Description ?? string.Empty,
                ImageUrl = model.ImageUrl ?? string.Empty
            }
        };
    }
}
