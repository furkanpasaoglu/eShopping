using Payment.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace Payment.Application.Commands.RefundPayment;

public sealed record RefundPaymentCommand(Guid PaymentId) : ICommand<PaymentResponse>;
