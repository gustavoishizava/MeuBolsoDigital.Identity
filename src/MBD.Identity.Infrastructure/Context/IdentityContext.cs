using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using MBD.Identity.Domain.Entities;
using MongoDB.Driver;

namespace MBD.Identity.Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class IdentityContext : DbContext
    {
        public IdentityContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options) : base(mongoClient, mongoDatabase, options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}