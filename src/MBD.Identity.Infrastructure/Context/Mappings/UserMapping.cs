using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.Identity.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.Identity.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class UserMapping : BsonClassMapConfiguration
    {
        public UserMapping() : base("users")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<User>();

            map.MapProperty(x => x.Name)
                    .SetElementName("name");

            map.MapProperty(x => x.Email)
                .SetElementName("email");

            map.MapProperty(x => x.Password)
                .SetElementName("password");

            return map;
        }
    }
}