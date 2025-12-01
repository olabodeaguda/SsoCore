using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SsoCore.Application.Handlers.Behaviours;
using SsoCore.Application.Validations;

namespace SsoCore.Application.Configurations
{
    public static class ConfigurationExtensions
    {
        public static void AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(_ =>
            {
                _.RegisterServicesFromAssemblies(new[] { typeof(ConfigurationExtensions).Assembly })
                    .AddOpenBehavior(typeof(LoggingBehaviour<,>))
                    .AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });
            services.AddValidatorsFromAssemblyContaining<CreateClientRequestValidation>();

            services.AddMemoryCache();
            services.AddSingleton<ConfigSettings>();
        }
    }
}
