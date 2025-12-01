using MassTransit.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Quartz;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using SsoCore.Infrastructure.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace SsoCore.Provider.Configurations
{
    public static class Extension
    {
        public static void AddOpenIdConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            string? path = configuration.GetValue<string>("Certificate:Path");
            string? secret = configuration.GetValue<string>("Certificate:Secret");

            string certificateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(), path!.Trim());

            if (string.IsNullOrEmpty(path)
               || string.IsNullOrEmpty(secret)
               || !File.Exists(certificateDirectory)) throw new Exception($"Signed certificate has not been configured => {certificateDirectory}");

            var certificate = X509CertificateLoader.LoadPkcs12FromFile(certificateDirectory, secret);

            services.AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                        .DisableBulkOperations()
                        .UseDbContext<ApplicationDbContext>();

                options.UseQuartz();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("connect/authorize")
                      .SetEndSessionEndpointUris("connect/logout")
                      .SetTokenEndpointUris("connect/token")
                       .SetIntrospectionEndpointUris("connect/introspect")
                       .SetEndUserVerificationEndpointUris("connect/verify")
                      .SetUserInfoEndpointUris("connect/userinfo");

                options.AllowAuthorizationCodeFlow()
                .AllowHybridFlow()
                .AllowClientCredentialsFlow()
                .AllowPasswordFlow()
                .AllowRefreshTokenFlow();

                options.DisableAccessTokenEncryption()
                .AddEncryptionCertificate(certificate)
                 .AddSigningCertificate(certificate);

                options.AddEventHandler<HandleIntrospectionRequestContext>(b =>
                {
                    b.UseInlineHandler(context =>
                    {
                        context.Claims[Claims.Email] = context.Principal.Claims.FirstOrDefault(x => x.Type == Claims.Email)?.Value;
                        context.Claims[Claims.Name] = context.Principal.Claims.FirstOrDefault(x => x.Type == Claims.Name)?.Value;
                        context.Claims[ClaimTypes.Email] = context.Principal.Claims.FirstOrDefault(x => x.Type == Claims.Email)?.Value;
                        context.Claims[ClaimTypes.Name] = context.Principal.Claims.FirstOrDefault(x => x.Type == Claims.Name)?.Value;
                        context.Claims[ClaimTypes.NameIdentifier] = context.Principal.Claims.FirstOrDefault(x => x.Type == Claims.Subject)?.Value;
                        return default;
                    });
                });

                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                options.UseAspNetCore()
                     .EnableStatusCodePagesIntegration()
                     .EnableAuthorizationEndpointPassthrough()
                     .EnableEndSessionEndpointPassthrough()
                     .EnableTokenEndpointPassthrough()
                     .EnableUserInfoEndpointPassthrough()
                     .EnableEndUserVerificationEndpointPassthrough();
            })
            .AddClient(options =>
            {
                options.AllowAuthorizationCodeFlow();
                options.UseAspNetCore()
               .EnableRedirectionEndpointPassthrough();

                options
               .AddEncryptionCertificate(certificate)
                .AddSigningCertificate(certificate);

                options.UseWebProviders()
                .AddGoogle(g =>
                {
                    g.SetClientId(configuration.GetValue<string>("Authentication:Google:ClientId")?.Trim() ?? string.Empty);
                    g.SetClientSecret(configuration.GetValue<string>("Authentication:Google:ClientSecret")?.Trim() ?? string.Empty);
                    g.SetRedirectUri($"callback/login/{Providers.Google}");
                    g.SetProviderDisplayName(Providers.Google);
                    g.AddScopes(Scopes.Email, Scopes.Profile, Scopes.OpenId);
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(c =>
                {
                    c.LoginPath = "/";
                });
        }
    }
}
