using Catalog.Application.ReadModels;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Persistence.Elasticsearch;

internal sealed class CatalogElasticsearchInitializer(
    ElasticsearchClient client,
    ILogger<CatalogElasticsearchInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var existsResponse = await client.Indices.ExistsAsync(
            CatalogIndexMapping.IndexName, cancellationToken);

        if (existsResponse.Exists)
        {
            logger.LogInformation(
                "Elasticsearch index '{Index}' already exists.", CatalogIndexMapping.IndexName);
            return;
        }

        var createResponse = await client.Indices.CreateAsync<ProductReadModel>(
            CatalogIndexMapping.IndexName,
            c => c.Mappings(m => m.Properties(p => p
                .Text(pr => pr.Name)
                .Keyword(pr => pr.Category)
                .Keyword(pr => pr.Currency)
                .Boolean(pr => pr.IsDeleted)
                .Date("createdAt")
                .Date("updatedAt")
            )),
            cancellationToken);

        if (createResponse.IsValidResponse)
            logger.LogInformation(
                "Created Elasticsearch index '{Index}'.", CatalogIndexMapping.IndexName);
        else
            logger.LogError(
                "Failed to create Elasticsearch index '{Index}': {Error}",
                CatalogIndexMapping.IndexName,
                createResponse.ElasticsearchServerError?.Error?.Reason);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
