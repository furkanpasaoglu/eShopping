using MediatR;
using Payment.Application.Commands.CapturePayment;
using Payment.Application.DTOs;
using Shared.BuildingBlocks.Extensions;

namespace Payment.API.Endpoints;

internal static class CapturePaymentEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/{id:guid}/capture", Handle)
            .WithName("CapturePayment")
            .WithSummary("Capture a reserved payment")
            .WithDescription("Captures funds that were previously reserved. Transitions payment from Reserved to Captured state.")
            .Produces<PaymentResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid id,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new CapturePaymentCommand(id), ct)).ToHttpResult();
}
