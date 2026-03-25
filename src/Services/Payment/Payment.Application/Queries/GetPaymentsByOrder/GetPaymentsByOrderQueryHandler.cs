using Mapster;
using Payment.Application.Abstractions;
using Payment.Application.DTOs;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Results;

namespace Payment.Application.Queries.GetPaymentsByOrder;

internal sealed class GetPaymentsByOrderQueryHandler(IPaymentRepository paymentRepository)
    : IQueryHandler<GetPaymentsByOrderQuery, IReadOnlyList<PaymentResponse>>
{
    public async Task<Result<IReadOnlyList<PaymentResponse>>> Handle(
        GetPaymentsByOrderQuery request, CancellationToken cancellationToken)
    {
        var payments = await paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);

        return payments.Adapt<List<PaymentResponse>>().AsReadOnly();
    }
}
