using Payment.Domain.ValueObjects;
using Shared.BuildingBlocks.Domain.Events;

namespace Payment.Domain.Events;

public sealed record PaymentCapturedDomainEvent(PaymentId PaymentId, Guid OrderId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
