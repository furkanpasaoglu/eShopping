using Payment.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace Payment.Application.Commands.ReservePayment;

public sealed record ReservePaymentCommand(
    Guid OrderId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    string? CardNumber = null,
    string? ExpiryMonth = null,
    string? ExpiryYear = null,
    string? CardHolderName = null) : ICommand<PaymentResponse>;
