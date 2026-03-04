using MongoDB.Bson.Serialization;

namespace Catalog.Infrastructure.Persistence;

internal static class BsonMappingConfig
{
    private static bool _registered;
    private static readonly object _lock = new();

    public static void Register()
    {
        lock (_lock)
        {
            if (_registered) return;
            _registered = true;

            BsonClassMap.RegisterClassMap<ProductDocument>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
