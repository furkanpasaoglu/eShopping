using Catalog.API.Endpoints;
using Catalog.API.Grpc;
using Catalog.Application;
using Catalog.Infrastructure;
using Scalar.AspNetCore;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGrpc();
builder.Services.AddOpenApi();
builder.Services.AddServiceAuthentication();
builder.Services.AddAuthorizationPolicies();
builder.Services.AddCurrentUser();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => { options.Title = "Catalog API"; });
app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<CatalogGrpcService>();

app.MapGroup("/api/v1/catalog")
    .WithTags("Catalog")
    .MapProductEndpoints();

app.Run();

public partial class Program;
