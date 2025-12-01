using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SsoCore.Application.Interfaces.Repositories;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Data;
using SsoCore.Infrastructure.Data.Identity;
using SsoCore.Infrastructure.Data.Repositories;
using SsoCore.Infrastructure.Services;

namespace SsoCore.Infrastructure.Configurations
{
    public static class ConfigurationExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AppDatabaseConfiguration(configuration);
            services.AddMessageBrokerConfiguration(configuration);
            services.AddDIConfiguration();
        }

        public static void AddDIConfiguration(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IScopeService, ScopeService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
        }

        public static void AppDatabaseConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            services.AddDbContextPool<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder.UseMySql(connectionString,serverVersion);
                optionsBuilder.UseOpenIddict();
            });
        }

        public static void AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
        }

        public static void AddMessageBrokerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            RabbitMQSettings rabbitMqSettings = configuration.GetSection("RabbitMqSettings").Get<RabbitMQSettings>()!;

            services.AddSingleton(rabbitMqSettings);
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((cxt, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.HostName, rabbitMqSettings.VHost, h =>
                    {
                        h.Username(rabbitMqSettings.UserName);
                        h.Password(rabbitMqSettings.Password);
                    });
                });
            });

            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromSeconds(10);
                    options.StopTimeout = TimeSpan.FromSeconds(30);
                });
        }
    }
}
