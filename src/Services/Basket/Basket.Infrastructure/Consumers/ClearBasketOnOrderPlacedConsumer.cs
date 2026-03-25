using Basket.Application.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events.Orders;

namespace Basket.Infrastructure.Consumers;

/// <summary>
/// Clears the user's shopping basket after an order is successfully placed.
/// Subscribes to <see cref="OrderPlacedIntegrationEvent"/> independently from the Order saga.
/// </summary>
public sealed class ClearBasketOnOrderPlacedConsumer(
    IBasketRepository basketRepository,
    ILogger<ClearBasketOnOrderPlacedConsumer> logger) : IConsumer<OrderPlacedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedIntegrationEvent> context)
    {
        var username = context.Message.Username;

        logger.LogInformation(
            "Clearing basket for user {Username} after order {OrderId} placed",
            username, context.Message.OrderId);

        await basketRepository.DeleteAsync(username, context.CancellationToken);

        logger.LogInformation("Basket cleared for user {Username}", username);
    }
}
