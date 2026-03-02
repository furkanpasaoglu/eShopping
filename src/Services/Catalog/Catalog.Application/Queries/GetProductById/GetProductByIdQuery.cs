using Catalog.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace Catalog.Application.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
