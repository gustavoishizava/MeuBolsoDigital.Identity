using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Extensions;
using MBD.Identity.Infrastructure.Context;
using MBD.Identity.Infrastructure.Context.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Identity.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext<IdentityContext>(options =>
            {
                options.ConfigureConnection(configuration.GetConnectionString("Default"), configuration["DatabaseName"]);
                options.AddSerializer(new GuidSerializer(BsonType.String));

                options.AddBsonClassMap(new BaseEntityMapping());
                options.AddBsonClassMap(new EmailMapping());
                options.AddBsonClassMap(new StrongPasswordMapping());
                options.AddBsonClassMap(new UserMapping());
            });

            return services;
        }
    }
}