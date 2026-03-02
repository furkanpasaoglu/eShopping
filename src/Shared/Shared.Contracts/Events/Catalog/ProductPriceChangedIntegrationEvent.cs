using Shared.Contracts.Events;

namespace Shared.Contracts.Events.Catalog;

public sealed record ProductPriceChangedIntegrationEvent(
    Guid ProductId,
    decimal OldPrice,
    decimal NewPrice,
    string Currency) : IntegrationEvent;
