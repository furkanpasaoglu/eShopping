using Catalog.Application.Abstractions;
using Catalog.Domain.ValueObjects;
using Mapster;
using Catalog.Application.ReadModels;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events.Stock;

namespace Catalog.Infrastructure.Consumers;

public sealed class StockUpdatedConsumer(
    IProductWriteRepository writeRepository,
    IProductReadRepository readRepository,
    ILogger<StockUpdatedConsumer> logger) : IConsumer<StockUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StockUpdatedIntegrationEvent> context)
    {
        foreach (var item in context.Message.Items)
        {
            var product = await writeRepository.GetByIdAsync(
                ProductId.From(item.ProductId), context.CancellationToken);

            if (product is null)
            {
                logger.LogWarning(
                    "Product {ProductId} not found while syncing stock delta {Delta}",
                    item.ProductId, item.Delta);
                continue;
            }

            var result = product.AdjustStock(item.Delta);
            if (result.IsFailure)
            {
                logger.LogWarning(
                    "Failed to adjust stock for product {ProductId} with delta {Delta}",
                    item.ProductId, item.Delta);
                continue;
            }

            await writeRepository.UpdateAsync(product, context.CancellationToken);
            await readRepository.UpsertAsync(product.Adapt<ProductReadModel>(), context.CancellationToken);

            logger.LogInformation(
                "Catalog stock synced for product {ProductId}: delta={Delta}, new={NewStock}",
                item.ProductId, item.Delta, product.Stock.Value);
        }
    }
}
