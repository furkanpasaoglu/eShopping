using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Payment API";
});

var group = app.MapGroup("/api/v1/payment").WithTags("Payment");

group.MapPost("/process", (ProcessPaymentRequest request) =>
    TypedResults.Ok(new PaymentResponse(Guid.NewGuid(), request.OrderId, "Approved", DateTime.UtcNow)))
    .WithName("ProcessPayment");

app.Run();

record ProcessPaymentRequest(Guid OrderId, decimal Amount, string CardHolder, string CardNumber);
record PaymentResponse(Guid PaymentId, Guid OrderId, string Status, DateTime ProcessedAt);
