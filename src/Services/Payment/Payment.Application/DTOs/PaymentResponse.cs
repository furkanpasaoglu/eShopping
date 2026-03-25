namespace Payment.Application.DTOs;

public sealed record PaymentResponse(
    Guid Id,
    Guid OrderId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    string Status,
    string? MaskedCardNumber,
    string? TransactionId,
    string? FailureReason,
    DateTimeOffset ReservedAt,
    DateTimeOffset? CapturedAt,
    DateTimeOffset? RefundedAt);
