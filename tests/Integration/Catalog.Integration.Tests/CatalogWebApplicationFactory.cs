using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Testcontainers.Elasticsearch;
using Testcontainers.MongoDb;

namespace Catalog.Integration.Tests;

public sealed class CatalogWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder().Build();
    private readonly ElasticsearchContainer _elasticsearchContainer = new ElasticsearchBuilder().Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _mongoContainer.StartAsync(),
            _elasticsearchContainer.StartAsync());
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _mongoContainer.StopAsync(),
            _elasticsearchContainer.StopAsync());
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IMongoClient>();
            services.RemoveAll<IMongoDatabase>();
            services.RemoveAll<ElasticsearchClient>();

            var mongoConnectionString = _mongoContainer.GetConnectionString();
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
            services.AddSingleton(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase("catalog-test"));

            var esUri = new Uri(_elasticsearchContainer.GetConnectionString());
            services.AddSingleton<ElasticsearchClient>(_ => new ElasticsearchClient(esUri));
        });

        builder.UseEnvironment("Testing");
    }
}
