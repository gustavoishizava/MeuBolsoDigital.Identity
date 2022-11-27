using DotNet.MongoDB.Context.Mapping;
using MBD.Identity.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.Identity.Infrastructure.Context.Mappings
{
    public sealed class BaseEntityMapping : BsonClassMapConfiguration
    {
        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<BaseEntity>();

            map.MapIdProperty(x => x.Id);

            map.MapProperty(x => x.CreatedAt)
                .SetElementName("created_at");

            map.MapProperty(x => x.UpdatedAt)
                .SetElementName("updated_at");

            return map;
        }
    }
}