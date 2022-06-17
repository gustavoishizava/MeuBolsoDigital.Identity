using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MBD.Identity.API.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddEFContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}