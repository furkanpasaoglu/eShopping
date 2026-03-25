using Payment.Domain.Enums;
using Payment.Domain.Errors;
using Payment.Domain.Events;
using Payment.Domain.ValueObjects;
using Shared.BuildingBlocks.Domain.Primitives;
using Shared.BuildingBlocks.Results;

namespace Payment.Domain.Entities;

public sealed class Payment : AggregateRoot<PaymentId>, IAuditableEntity
{
    private Payment() : base(PaymentId.New()) { }

    private Payment(
        PaymentId id,
        Guid orderId,
        Guid customerId,
        Money amount,
        CardInfo card,
        string transactionId) : base(id)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Amount = amount;
        Card = card;
        TransactionId = transactionId;
        Status = PaymentStatus.Reserved;
        ReservedAt = DateTimeOffset.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public CardInfo Card { get; private set; } = null!;
    public PaymentStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    public string TransactionId { get; private set; } = string.Empty;
    public DateTimeOffset ReservedAt { get; private set; }
    public DateTimeOffset? CapturedAt { get; private set; }
    public DateTimeOffset? RefundedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Result<Payment> Reserve(
        Guid orderId,
        Guid customerId,
        Money amount,
        CardInfo card,
        string transactionId)
    {
        if (amount.Amount <= 0)
            return PaymentErrors.InvalidAmount;

        var payment = new Payment(PaymentId.New(), orderId, customerId, amount, card, transactionId);
        payment.RaiseDomainEvent(new PaymentReservedDomainEvent(payment.Id, orderId, amount.Amount));

        return payment;
    }

    public static Payment CreateFailed(
        Guid orderId,
        Guid customerId,
        Money amount,
        CardInfo card,
        string transactionId,
        string reason)
    {
        var payment = new Payment(PaymentId.New(), orderId, customerId, amount, card, transactionId)
        {
            Status = PaymentStatus.Failed,
            FailureReason = reason
        };
        payment.RaiseDomainEvent(new PaymentFailedDomainEvent(payment.Id, orderId, reason));

        return payment;
    }

    public Result Capture()
    {
        if (Status != PaymentStatus.Reserved)
            return PaymentErrors.CannotCaptureUnreserved;

        Status = PaymentStatus.Captured;
        CapturedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentCapturedDomainEvent(Id, OrderId));

        return Result.Success();
    }

    public Result Refund()
    {
        if (Status != PaymentStatus.Captured)
            return PaymentErrors.CannotRefundUncaptured;

        Status = PaymentStatus.Refunded;
        RefundedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentRefundedDomainEvent(Id, OrderId));

        return Result.Success();
    }
}
