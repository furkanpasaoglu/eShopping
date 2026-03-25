using Mapster;
using Payment.Application.Abstractions;
using Payment.Application.DTOs;
using Payment.Domain.Errors;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Results;

namespace Payment.Application.Commands.CapturePayment;

internal sealed class CapturePaymentCommandHandler(IPaymentRepository paymentRepository)
    : ICommandHandler<CapturePaymentCommand, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
            return PaymentErrors.NotFound;

        var result = payment.Capture();

        if (result.IsFailure)
            return result.Error;

        await paymentRepository.SaveChangesAsync(cancellationToken);

        return payment.Adapt<PaymentResponse>();
    }
}
