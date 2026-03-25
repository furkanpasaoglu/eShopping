using Mapster;
using Microsoft.Extensions.Logging;
using Payment.Application.Abstractions;
using Payment.Application.DTOs;
using Payment.Domain.ValueObjects;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Results;

namespace Payment.Application.Commands.ReservePayment;

internal sealed class ReservePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IPaymentGateway paymentGateway,
    ILogger<ReservePaymentCommandHandler> logger)
    : ICommandHandler<ReservePaymentCommand, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(ReservePaymentCommand request, CancellationToken cancellationToken)
    {
        var gatewayResult = await paymentGateway.ProcessAsync(
            request.OrderId, request.Amount, request.Currency, cancellationToken);

        var money = Money.Create(request.Amount, request.Currency);
        var card = CardInfo.Create(request.CardNumber, request.ExpiryMonth, request.ExpiryYear, request.CardHolderName);

        if (!gatewayResult.Success)
        {
            var failedPayment = Payment.Domain.Entities.Payment.CreateFailed(
                request.OrderId, request.CustomerId, money, card,
                gatewayResult.TransactionId, gatewayResult.FailureReason ?? "Payment declined");

            await paymentRepository.AddAsync(failedPayment, cancellationToken);
            await paymentRepository.SaveChangesAsync(cancellationToken);

            logger.LogWarning("Payment failed for order {OrderId}: {Reason}",
                request.OrderId, gatewayResult.FailureReason);

            return Error.Failure("Payment.Declined", gatewayResult.FailureReason ?? "Payment declined");
        }

        var result = Payment.Domain.Entities.Payment.Reserve(
            request.OrderId, request.CustomerId, money, card, gatewayResult.TransactionId);

        if (result.IsFailure)
            return result.Error;

        await paymentRepository.AddAsync(result.Value, cancellationToken);
        await paymentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Payment reserved for order {OrderId}, transaction {TransactionId}",
            request.OrderId, gatewayResult.TransactionId);

        return result.Value.Adapt<PaymentResponse>();
    }
}
