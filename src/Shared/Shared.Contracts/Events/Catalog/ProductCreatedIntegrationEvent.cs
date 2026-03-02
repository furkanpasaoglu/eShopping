using Shared.Contracts.Events;

namespace Shared.Contracts.Events.Catalog;

public sealed record ProductCreatedIntegrationEvent(
    Guid ProductId,
    string Name,
    string Category,
    decimal Price,
    string Currency,
    int Stock) : IntegrationEvent;
