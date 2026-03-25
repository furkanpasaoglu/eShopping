using Shared.Contracts.Events;

namespace Shared.Contracts.Events.Stock;

/// <summary>
/// Published by the Stock service after stock quantities change (reserve or release).
/// Consumed by the Catalog service to keep its read-model stock values in sync.
/// </summary>
public sealed record StockUpdatedIntegrationEvent(
    IReadOnlyList<StockUpdateItem> Items) : IntegrationEvent;

public sealed record StockUpdateItem(Guid ProductId, int Delta);
