using Catalog.Domain.ValueObjects;
using Shared.BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

public sealed record StockAdjustedDomainEvent(
    ProductId ProductId,
    int Delta,
    int NewQuantity) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
