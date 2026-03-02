using Catalog.Application.Commands.AdjustStock;
using Catalog.Application.DTOs;
using MediatR;
using Shared.BuildingBlocks.Extensions;

namespace Catalog.API.Endpoints;

internal static class AdjustStockEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPatch("/products/{id:guid}/stock", Handle)
            .WithName("AdjustStock")
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(409)
            .RequireAuthorization("RequireAdmin");

    private static async Task<IResult> Handle(
        Guid id,
        AdjustStockRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new AdjustStockCommand(id, request.Delta), ct);

        if (result.IsFailure)
            return result.ToHttpResult();

        return Results.NoContent();
    }
}
