using Shared.BuildingBlocks.CQRS;

namespace Catalog.Application.Commands.AdjustStock;

public sealed record AdjustStockCommand(Guid ProductId, int Delta) : ICommand;
