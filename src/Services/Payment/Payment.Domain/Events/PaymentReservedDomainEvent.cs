using Payment.Domain.ValueObjects;
using Shared.BuildingBlocks.Domain.Events;

namespace Payment.Domain.Events;

public sealed record PaymentReservedDomainEvent(PaymentId PaymentId, Guid OrderId, decimal Amount) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
