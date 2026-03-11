using Basket.Application.Abstractions;
using Basket.Infrastructure.Grpc;
using Basket.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Grpc.Catalog;

namespace Basket.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.AddRedisClient("redis");

        builder.Services.Configure<BasketOptions>(
            configuration.GetSection(BasketOptions.SectionName));

        builder.Services.AddGrpcClient<CatalogGrpcService.CatalogGrpcServiceClient>(o =>
            o.Address = new Uri("https+http2://catalog-api"))
            .AddStandardResilienceHandler();

        builder.Services.AddScoped<IBasketRepository, BasketRedisRepository>();
        builder.Services.AddScoped<ICatalogGrpcClient, CatalogGrpcClient>();

        return builder;
    }
}
