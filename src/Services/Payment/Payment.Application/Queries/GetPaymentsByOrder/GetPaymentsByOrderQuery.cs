using Payment.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace Payment.Application.Queries.GetPaymentsByOrder;

public sealed record GetPaymentsByOrderQuery(Guid OrderId) : IQuery<IReadOnlyList<PaymentResponse>>;
