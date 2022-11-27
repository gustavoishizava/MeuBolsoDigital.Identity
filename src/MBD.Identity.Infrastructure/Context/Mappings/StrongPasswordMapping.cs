using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.Identity.Domain.ValueObjects;
using MongoDB.Bson.Serialization;

namespace MBD.Identity.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class StrongPasswordMapping : BsonClassMapConfiguration
    {
        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<StrongPassword>();

            map.MapProperty(x => x.PasswordHash)
                    .SetElementName("hash");

            return map;
        }
    }
}