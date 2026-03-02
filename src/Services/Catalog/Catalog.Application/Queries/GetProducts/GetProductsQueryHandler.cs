using Catalog.Application.Abstractions;
using Catalog.Application.DTOs;
using Mapster;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Pagination;
using Shared.BuildingBlocks.Results;

namespace Catalog.Application.Queries.GetProducts;

internal sealed class GetProductsQueryHandler(IProductReadRepository readRepository)
    : IQueryHandler<GetProductsQuery, PagedList<ProductResponse>>
{
    public async Task<Result<PagedList<ProductResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await readRepository.GetPagedAsync(
            request.Category,
            request.Name,
            request.MinPrice,
            request.MaxPrice,
            request.Pagination,
            cancellationToken);

        var responses = items.Select(m => m.Adapt<ProductResponse>()).ToList().AsReadOnly();
        var paged = PagedList<ProductResponse>.Create(
            responses,
            request.Pagination.Page,
            request.Pagination.PageSize,
            totalCount);

        return paged;
    }
}
