using Catalog.Application.ReadModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

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

            BsonClassMap.RegisterClassMap<ProductReadModel>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdProperty(r => r.Id)
                    .SetSerializer(new GuidSerializer(BsonType.String));
            });

            BsonClassMap.RegisterClassMap<ProductDocument>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
