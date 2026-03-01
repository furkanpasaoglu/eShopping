using Scalar.AspNetCore;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Basket API";
});
app.MapDefaultEndpoints();

var group = app.MapGroup("/api/v1/basket").WithTags("Basket");

group.MapGet("/{username}", (string username) =>
    TypedResults.Ok(new BasketResponse(
        username,
        new[]
        {
            new BasketItem("prod-1", "Laptop Pro 15", 1, 1299.99m),
            new BasketItem("prod-2", "Wireless Mouse", 2, 29.99m)
        },
        1359.97m)))
    .WithName("GetBasket");

group.MapPost("/", (UpdateBasketRequest request) =>
    TypedResults.Ok(new BasketResponse(
        request.Username,
        request.Items,
        request.Items.Sum(i => i.Price * i.Quantity))))
    .WithName("UpdateBasket");

group.MapDelete("/{username}", (string username) =>
    TypedResults.NoContent())
    .WithName("DeleteBasket");

app.Run();

record BasketItem(string ProductId, string ProductName, int Quantity, decimal Price);
record BasketResponse(string Username, IEnumerable<BasketItem> Items, decimal TotalPrice);
record UpdateBasketRequest(string Username, BasketItem[] Items);
