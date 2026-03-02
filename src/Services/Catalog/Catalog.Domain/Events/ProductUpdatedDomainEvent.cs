using Catalog.Domain.ValueObjects;
using Shared.BuildingBlocks.Domain.Events;

namespace Catalog.Domain.Events;

public sealed record ProductUpdatedDomainEvent(ProductId ProductId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
