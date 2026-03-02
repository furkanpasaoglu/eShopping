using Shared.BuildingBlocks.CQRS;

namespace Catalog.Application.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : ICommand;
