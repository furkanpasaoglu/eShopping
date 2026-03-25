using MediatR;
using Payment.Application.DTOs;
using Payment.Application.Queries.GetPaymentById;
using Shared.BuildingBlocks.Extensions;

namespace Payment.API.Endpoints;

internal static class GetPaymentByIdEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/{id:guid}", Handle)
            .WithName("GetPaymentById")
            .WithSummary("Get payment by ID")
            .WithDescription("Returns detailed payment information including status, amount, and card details.")
            .Produces<PaymentResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new GetPaymentByIdQuery(id), ct)).ToHttpResult();
}
