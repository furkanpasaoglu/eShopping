using Payment.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace Payment.Application.Commands.CapturePayment;

public sealed record CapturePaymentCommand(Guid PaymentId) : ICommand<PaymentResponse>;
