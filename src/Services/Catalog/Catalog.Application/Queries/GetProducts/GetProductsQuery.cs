using Catalog.Application.DTOs;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Pagination;

namespace Catalog.Application.Queries.GetProducts;

public sealed record GetProductsQuery(
    string? Category,
    string? Name,
    decimal? MinPrice,
    decimal? MaxPrice,
    PaginationParams Pagination) : IQuery<PagedList<ProductResponse>>;
