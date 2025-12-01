using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using SsoCore.Infrastructure.Data;
using SsoCore.Infrastructure.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SsoCore.Provider.Configurations
{
    public static class SeedData
    {
        public static async Task<WebApplication> RunMigrationAsync(this WebApplication host)
        {
            using (var serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var clients = configuration.GetSection("Clients").Get<Client[]>() ?? [];
                    var scopes = clients
                      .SelectMany(client => client.Scopes?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [])
                      .Select(scope => scope.Trim())
                      .Where(scope => !string.IsNullOrWhiteSpace(scope))
                      .Distinct()
                      .ToArray();

                    await SeedScopesAsync(serviceScope, scopes);
                    await SeedClientsAsync(serviceScope, clients);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error at {TypeName}=> RunMigrations", typeof(SeedData).FullName);
                }
            }

            return host;
        }

        private static async Task SeedScopesAsync(IServiceScope serviceScope, string[] scopes)
        {
            var openIdScopeManager = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            var clientScopes = scopes
                .Where(_ => !string.IsNullOrWhiteSpace(_) || !string.IsNullOrEmpty(_))
                .Select(x =>
                    new OpenIddictScopeDescriptor
                    {
                        Name = x,
                        Resources = { x }
                    }).ToArray();

            foreach (var scope in clientScopes)
            {
                var scopeExists = await openIdScopeManager.FindByNameAsync(scope.Name!);
                if (scopeExists == null)
                    await openIdScopeManager.CreateAsync(scope).AsTask().ConfigureAwait(false);
            }
        }

        private static async Task SeedClientsAsync(IServiceScope serviceScope, Client[] clients)
        {
            var openIdClientManager = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            foreach (var client in clients)
            {
                var clientExists = await openIdClientManager.FindByClientIdAsync(client.ClientId!);
                if (clientExists == null)
                {
                    var descriptor = OpenIddictApplicationDescriptor(client);
                    await openIdClientManager.CreateAsync(descriptor);
                }
            }
        }

        private static OpenIddictApplicationDescriptor OpenIddictApplicationDescriptor(Client client)
        {
            var application = new OpenIddictApplicationDescriptor
            {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret,
                DisplayName = client.DisplayName,
                ClientType = ClientTypes.Confidential,
                ConsentType = ConsentTypes.Explicit,
                ApplicationType = ApplicationTypes.Web,
                Permissions =
                            {
                                Permissions.Endpoints.Authorization,
                                Permissions.Endpoints.EndSession,
                                Permissions.Endpoints.Token,
                                Permissions.Endpoints.Introspection,
                                Permissions.Endpoints.DeviceAuthorization,
                                Permissions.GrantTypes.RefreshToken,
                                Permissions.ResponseTypes.Code,
                                Permissions.Scopes.Email,
                                Permissions.Scopes.Profile,
                                Permissions.GrantTypes.ClientCredentials,
                                Scopes.OfflineAccess
                            },
                Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
            };

            var scopes = client.Scopes?.Split([',']) ?? [];
            foreach (var scope in scopes)
            {
                application.Permissions.Add($"{OpenIddictConstants.Permissions.Prefixes.Scope}{scope.Trim()}");
            }

            var logoutRedirectUris = client.PostLogoutRedirectUri?.Split([',']) ?? [];
            if (logoutRedirectUris.Length > 0)
                foreach (var item in logoutRedirectUris)
                    application.PostLogoutRedirectUris.Add(new Uri(item));

            var redirectUri = client.RedirectUri?.Split([',']) ?? [];
            if (redirectUri.Length > 0)
                foreach (var item in redirectUri)
                    application.RedirectUris.Add(new Uri(item));

            return application;
        }
    }
}
