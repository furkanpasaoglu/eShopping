using MediatR;
using Payment.Application.Commands.RefundPayment;
using Payment.Application.DTOs;
using Shared.BuildingBlocks.Extensions;

namespace Payment.API.Endpoints;

internal static class RefundPaymentEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/{id:guid}/refund", Handle)
            .WithName("RefundPayment")
            .WithSummary("Refund a captured payment")
            .WithDescription("Refunds a previously captured payment. Transitions payment from Captured to Refunded state.")
            .Produces<PaymentResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid id,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new RefundPaymentCommand(id), ct)).ToHttpResult();
}
