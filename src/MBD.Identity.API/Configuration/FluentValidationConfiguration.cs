using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using System.Diagnostics.CodeAnalysis;

namespace MBD.Identity.API.Configuration
{
    [ExcludeFromCodeCoverageAttribute]
    public static class FluentValidationConfiguration
    {
        public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidation(options =>
                options.RegisterValidatorsFromAssembly(Assembly.Load("MBD.Identity.Application")));

            services.AddFluentValidationRulesToSwagger();

            return services;
        }
    }
}