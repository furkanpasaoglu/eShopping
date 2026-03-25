using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Seeding;
using Stock.Domain.Entities;
using Stock.Infrastructure.Persistence;

namespace Stock.Infrastructure.Seeding;

/// <summary>
/// Seeds initial stock quantities for catalog seed products.
/// Runs after <see cref="StockDbInitializer"/> ensures the schema exists.
/// Idempotent: skips if stock items already exist.
/// </summary>
internal sealed class StockDataSeeder(
    IServiceProvider serviceProvider,
    ILogger<StockDataSeeder> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                await SeedAsync(db, cancellationToken);
                return;
            }
            catch (Exception ex) when (attempt < 5)
            {
                var delay = TimeSpan.FromSeconds(attempt * 2);
                logger.LogWarning(ex,
                    "Stock seeding attempt {Attempt}/5 failed. Retrying in {Delay}s.",
                    attempt, delay.TotalSeconds);
                await Task.Delay(delay, cancellationToken);
            }
        }

        logger.LogError("Stock seeding failed after 5 attempts.");
    }

    private async Task SeedAsync(StockDbContext db, CancellationToken ct)
    {
        var existingCount = await db.StockItems.CountAsync(ct);
        if (existingCount > 0)
        {
            logger.LogInformation("Stock already seeded with {Count} items. Skipping.", existingCount);
            return;
        }

        logger.LogInformation("Seeding stock with catalog seed products...");

        foreach (var seed in SeedProductCatalog.Products)
        {
            var stockItem = StockItem.Create(seed.ProductId, seed.Stock);
            db.StockItems.Add(stockItem);
        }

        await db.SaveChangesAsync(ct);

        logger.LogInformation("Stock seeded with {Count} items.", SeedProductCatalog.Products.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
