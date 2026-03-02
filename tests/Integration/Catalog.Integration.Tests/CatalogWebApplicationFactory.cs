using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Catalog.Integration.Tests;

public sealed class CatalogWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder().Build();

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IMongoClient>();
            services.RemoveAll<IMongoDatabase>();

            var connectionString = _mongoContainer.GetConnectionString();
            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
            services.AddSingleton(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase("catalog-test"));
        });

        builder.UseEnvironment("Testing");
    }
}
