using MediatR;
using Payment.Application.DTOs;
using Payment.Application.Queries.GetPaymentsByOrder;
using Shared.BuildingBlocks.Extensions;

namespace Payment.API.Endpoints;

internal static class GetPaymentsByOrderEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/order/{orderId:guid}", Handle)
            .WithName("GetPaymentsByOrder")
            .WithSummary("Get payments by order ID")
            .WithDescription("Returns all payment records associated with a specific order.")
            .Produces<IReadOnlyList<PaymentResponse>>();

    private static async Task<IResult> Handle(
        Guid orderId,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new GetPaymentsByOrderQuery(orderId), ct)).ToHttpResult();
}
