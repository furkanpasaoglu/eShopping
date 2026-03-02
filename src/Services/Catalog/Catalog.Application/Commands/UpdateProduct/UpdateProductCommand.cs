using Shared.BuildingBlocks.CQRS;

namespace Catalog.Application.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    string Currency,
    string? Description,
    string? ImageUrl) : ICommand;
