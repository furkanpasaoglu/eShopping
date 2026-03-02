using Catalog.Domain.ValueObjects;
using Shared.BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

public sealed record ProductPriceChangedDomainEvent(
    ProductId ProductId,
    Money OldPrice,
    Money NewPrice) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
