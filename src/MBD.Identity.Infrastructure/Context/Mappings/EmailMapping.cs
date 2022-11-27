using DotNet.MongoDB.Context.Mapping;
using MBD.Identity.Domain.ValueObjects;
using MongoDB.Bson.Serialization;

namespace MBD.Identity.Infrastructure.Context.Mappings
{
    public sealed class EmailMapping : BsonClassMapConfiguration
    {
        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<Email>();

            map.MapProperty(x => x.Address)
                    .SetElementName("address");

            map.MapProperty(x => x.NormalizedAddress)
                .SetElementName("normalized_address");

            return map;
        }
    }
}