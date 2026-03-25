using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Payment.Infrastructure.Persistence;

internal sealed class PaymentDbInitializer(
    IServiceProvider serviceProvider,
    ILogger<PaymentDbInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                await db.Database.EnsureCreatedAsync(cancellationToken);
                logger.LogInformation("Payment database schema ready.");
                return;
            }
            catch (Exception ex) when (attempt < 5)
            {
                var delay = TimeSpan.FromSeconds(attempt * 2);
                logger.LogWarning(ex,
                    "Payment database initialization attempt {Attempt}/5 failed. Retrying in {Delay}s.",
                    attempt, delay.TotalSeconds);
                await Task.Delay(delay, cancellationToken);
            }
        }

        logger.LogError("Payment database initialization failed after 5 attempts.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
