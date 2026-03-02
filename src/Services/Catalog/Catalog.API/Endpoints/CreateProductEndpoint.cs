using Catalog.Application.Commands.CreateProduct;
using Catalog.Application.DTOs;
using MediatR;
using Shared.BuildingBlocks.Extensions;

namespace Catalog.API.Endpoints;

internal static class CreateProductEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/products", Handle)
            .WithName("CreateProduct")
            .Produces<Guid>(201)
            .ProducesProblem(400)
            .ProducesProblem(422)
            .RequireAuthorization("RequireAdmin");

    private static async Task<IResult> Handle(
        CreateProductRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Category,
            request.Price,
            request.Currency,
            request.Stock,
            request.Description,
            request.ImageUrl);

        var result = await sender.Send(command, ct);

        if (result.IsFailure)
            return result.ToHttpResult();

        return Results.Created($"/api/v1/catalog/products/{result.Value}", result.Value);
    }
}
