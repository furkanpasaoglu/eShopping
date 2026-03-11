namespace Basket.Infrastructure.Grpc;

public sealed class CatalogServiceUnavailableException(string message, Exception? inner = null)
    : Exception(message, inner);
