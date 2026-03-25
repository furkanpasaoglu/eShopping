namespace Payment.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,
    Reserved = 1,
    Captured = 2,
    Refunded = 3,
    Failed = 4
}
