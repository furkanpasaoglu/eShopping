using Mapster;
using Payment.Application.DTOs;

namespace Payment.Application.Mappings;

public static class PaymentMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Payment.Domain.Entities.Payment, PaymentResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.Amount, src => src.Amount.Amount)
            .Map(dest => dest.Currency, src => src.Amount.Currency)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.MaskedCardNumber, src => src.Card.MaskedNumber)
            .Map(dest => dest.TransactionId, src => src.TransactionId)
            .Map(dest => dest.FailureReason, src => src.FailureReason)
            .Map(dest => dest.ReservedAt, src => src.ReservedAt)
            .Map(dest => dest.CapturedAt, src => src.CapturedAt)
            .Map(dest => dest.RefundedAt, src => src.RefundedAt);
    }
}
