using Catalog.Application.Abstractions;
using Catalog.Application.DTOs;
using Catalog.Domain.Errors;
using Mapster;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Results;

namespace Catalog.Application.Queries.GetProductById;

internal sealed class GetProductByIdQueryHandler(IProductReadRepository readRepository)
    : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var model = await readRepository.GetByIdAsync(request.Id, cancellationToken);

        if (model is null || model.IsDeleted)
            return ProductErrors.NotFound;

        return model.Adapt<ProductResponse>();
    }
}
