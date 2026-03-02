using Shared.BuildingBlocks.CQRS;

namespace Catalog.Application.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Category,
    decimal Price,
    string Currency,
    int Stock,
    string? Description,
    string? ImageUrl) : ICommand<Guid>;
