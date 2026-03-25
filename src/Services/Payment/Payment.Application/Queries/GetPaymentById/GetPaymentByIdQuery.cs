using Payment.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace Payment.Application.Queries.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentResponse>;
