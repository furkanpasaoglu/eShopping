using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Order API";
});

var group = app.MapGroup("/api/v1/order").WithTags("Order");

group.MapGet("/{username}", (string username) =>
    TypedResults.Ok(new[]
    {
        new OrderResponse(Guid.NewGuid(), username, "Pending", 1359.97m, DateTime.UtcNow)
    }))
    .WithName("GetOrdersByUsername");

group.MapPost("/", (CreateOrderRequest request) =>
{
    var id = Guid.NewGuid();
    return TypedResults.Created($"/api/v1/order/{id}",
        new OrderResponse(id, request.Username, "Pending", request.TotalAmount, DateTime.UtcNow));
})
    .WithName("CreateOrder");

app.Run();

record OrderItem(string ProductId, string ProductName, int Quantity, decimal Price);
record OrderResponse(Guid Id, string Username, string Status, decimal TotalAmount, DateTime PlacedAt);
record CreateOrderRequest(string Username, OrderItem[] Items, decimal TotalAmount);
