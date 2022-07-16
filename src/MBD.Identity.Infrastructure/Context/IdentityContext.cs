using System;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using MBD.Identity.Domain.Entities;
using MBD.Identity.Domain.ValueObjects;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Identity.Infrastructure.Context
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(MongoDbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelConfiguring(ModelBuilder modelBuilder)
        {
            modelBuilder.AddModelMap<BaseEntity>("baseEntity", map =>
            {
                map.SetIsRootClass(true);

                map.MapIdProperty(x => x.Id);

                map.MapProperty(x => x.CreatedAt)
                    .SetElementName("created_at");

                map.MapProperty(x => x.UpdatedAt)
                    .SetElementName("updated_at");
            });

            modelBuilder.AddModelMap<Email>("email", map =>
            {
                map.MapProperty(x => x.Address)
                    .SetElementName("address");

                map.MapProperty(x => x.NormalizedAddress)
                    .SetElementName("normalized_address");
            });

            modelBuilder.AddModelMap<StrongPassword>("strongPassword", map =>
            {
                map.MapProperty(x => x.PasswordHash)
                    .SetElementName("hash");
            });

            modelBuilder.AddModelMap<User>("users", map =>
            {
                map.MapProperty(x => x.Name)
                    .SetElementName("name");

                map.MapProperty(x => x.Email)
                    .SetElementName("email");

                map.MapProperty(x => x.Password)
                    .SetElementName("password");
            });
        }
    }
}