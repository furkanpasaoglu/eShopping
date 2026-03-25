namespace Payment.Domain.ValueObjects;

public sealed record PaymentId(Guid Value)
{
    public static PaymentId New() => new(Guid.NewGuid());
    public static PaymentId From(Guid value) => new(value);
}
