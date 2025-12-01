using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;
using SsoCore.Infrastructure.Data.Identity;
using SsoCore.Provider.Helper;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SsoCore.Provider.Controllers
{
    [ApiController]
    public class AuthorizationController(IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IOpenIddictAuthorizationManager openIddictAuthorizationManager,
        ILogger<AuthorizationController> logger,
        IUserRoleService userRoleService) : ControllerBase
    {
        [HttpGet("~/callback/login/{provider}")]
        [HttpPost("~/callback/login/{provider}")]
        public async Task<IResult> ExternalLoginCallback(string provider)
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(provider);

                if (!result.Succeeded || result.Principal is null)
                {
                    return Results.BadRequest(Result.Fail(AuthorizationError.AuthorizationFailed()).Problem());
                }

                string? email = result.Principal.GetClaim(ClaimTypes.Email);
                string? providerKey = result.Principal.GetClaim(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerKey))
                {
                    return Results.BadRequest(Result.Fail(AuthorizationError.AuthorizationFailed()).Problem());
                }

                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        EmailConfirmed = true,
                        FirstName = result.Principal.GetClaim(ClaimTypes.GivenName),
                        LastName = result.Principal.GetClaim(ClaimTypes.Surname)
                    };

                    var userCreateResult = await userManager.CreateAsync(user);
                    if (!userCreateResult.Succeeded)
                    {
                        return Results.BadRequest(Result.Fail(AuthorizationError.AuthorizationFailed()).Problem());
                    }

                    await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));

                    await signInManager.SignInAsync(user, isPersistent: true);
                }
                else
                {
                    var signInResult = await signInManager.ExternalLoginSignInAsync(provider, providerKey, isPersistent: true, bypassTwoFactor: true);
                    if (!signInResult.Succeeded)
                    {
                        return Results.BadRequest(Result.Fail(AuthorizationError.AuthorizationFailed()).Problem());
                    }
                }

                var identity = new ClaimsIdentity(
                    authenticationType: "ExternalLogin",
                    nameType: Claims.Name,
                    roleType: Claims.Role
                );


                identity.SetClaim(Claims.Subject, user.Id)
                        .SetClaim(Claims.Email, user.Email)
                        .SetClaim(ClaimTypes.NameIdentifier, user.Id)
                        .SetClaim(Claims.Name, $"{user.FirstName} {user.MiddleNames} {user.LastName}".Trim())
                        .SetClaim(Claims.PreferredUsername, user.UserName);

                var registrationId = result.Principal.GetClaim(Claims.Private.RegistrationId);
                var providerName = result.Principal.GetClaim(Claims.Private.ProviderName);

                if (!string.IsNullOrEmpty(registrationId))
                {
                    identity.SetClaim(Claims.Private.RegistrationId, registrationId);
                }

                if (!string.IsNullOrEmpty(providerName))
                {
                    identity.SetClaim(Claims.Private.ProviderName, providerName);
                }

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return Results.Redirect(result.Properties?.RedirectUri ?? "/");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the external login callback.");
                return Results.BadRequest(Result.Fail(AuthorizationError.AuthorizationFailed()).Problem());
            }
        }

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        public async Task<IActionResult> Authorize()
        {
            try
            {

                var request = HttpContext.GetOpenIddictServerRequest() ??
                               throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

                var parameters = Helper.OAuthUtils.ParseOAuthParameters(HttpContext, [Parameters.Prompt]);

                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (!Helper.OAuthUtils.IsAuthenticated(result, request))
                {
                    return base.Challenge(properties: new AuthenticationProperties
                    {
                        RedirectUri = Helper.OAuthUtils.BuildRedirectUrl(HttpContext.Request, parameters)
                    }, [CookieAuthenticationDefaults.AuthenticationScheme]);
                }

                var user = await userManager.GetUserAsync(result.Principal ?? new ClaimsPrincipal()) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

                var application = await applicationManager.FindByClientIdAsync(request.ClientId ?? string.Empty) ??
                                  throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

                // Retrieve the permanent authorizations associated with the user and the calling client application.
                var authorizations = await openIddictAuthorizationManager.FindAsync(
                    subject: await userManager.GetUserIdAsync(user),
                    client: await applicationManager.GetIdAsync(application) ?? string.Empty,
                    status: Statuses.Valid,
                    type: AuthorizationTypes.Permanent,
                    scopes: request.GetScopes()).ToListAsync();

                var identity = await CreateClaimsIdentity(user, request);

                var authorization = authorizations.LastOrDefault();
                authorization ??= await openIddictAuthorizationManager.CreateAsync(
                    identity: identity,
                    subject: await userManager.GetUserIdAsync(user),
                    client: await applicationManager.GetIdAsync(application) ?? string.Empty,
                    type: AuthorizationTypes.Permanent,
                scopes: identity.GetScopes());

                identity.SetAuthorizationId(await openIddictAuthorizationManager.GetIdAsync(authorization));

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the authorization request.");
                return BadRequest(Result.Fail(AuthorizationError.AuthorizationFailed()).Problem());
            }
        }

        [HttpPost("~/connect/token")]
        [HttpGet("~/connect/token")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var grantType = request.GrantType;
            if (string.IsNullOrWhiteSpace(grantType))
                throw new InvalidOperationException("The grant type is missing from the request.");


            ClaimsPrincipal? principal = null;

            // Try to authenticate the principal from previous flow (e.g., code, refresh, password)
            if (!grantType!.Equals(OpenIddictConstants.GrantTypes.ClientCredentials, StringComparison.Ordinal))
            {
                var authenticateResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                principal = authenticateResult?.Principal
                    ?? throw new InvalidOperationException("The authentication principal cannot be retrieved.");
            }

            // Determine how to process based on grant type
            return grantType switch
            {
                GrantTypes.ClientCredentials => await HandleClientCredentialsGrantAsync(request),
                GrantTypes.AuthorizationCode => await HandleReissueGrantAsync(principal),
                GrantTypes.RefreshToken => await HandleReissueGrantAsync(principal),
                GrantTypes.Password => await HandleReissueGrantAsync(principal),

                _ => await HandleCustomGrantTypeAsync(request, principal)
            };
        }

        private Task<IActionResult> HandleClientCredentialsGrantAsync(OpenIddictRequest request)
        {
            // Basic client principal with its client_id
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType);
            var clientId = request.ClientId ?? throw new InvalidOperationException("Missing client ID for client credentials flow.");

            identity.SetClaim(OpenIddictConstants.Claims.Subject, clientId);
            identity.SetClaim(OpenIddictConstants.Claims.Name, clientId);
            identity.SetScopes(request.GetScopes());
            identity.SetResources("resource_server");

            var principal = new ClaimsPrincipal(identity);
            principal.SetDestinations(OAuthUtils.GetDestinations);

            return Task.FromResult<IActionResult>(
                SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
        }

        private Task<IActionResult> HandleReissueGrantAsync(ClaimsPrincipal principal)
        {
            // Re-issue access/ID/refresh tokens for known users
            principal.SetDestinations(OAuthUtils.GetDestinations);
            return Task.FromResult<IActionResult>(
                SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
        }

        private Task<IActionResult> HandleCustomGrantTypeAsync(OpenIddictRequest request, ClaimsPrincipal? principal)
        {
            // Custom handling fallback
            throw new NotSupportedException($"The grant type '{request.GrantType}' is not supported.");
        }

        [HttpGet("~/connect/logout")]
        [HttpPost("~/connect/logout")]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
        }

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo")]
        [IgnoreAntiforgeryToken, Produces("application/json")]
        public async Task<IActionResult> UserInfo()
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var userId = result?.Principal?.GetClaim(Claims.Subject);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge(
                                       authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                                                          properties: new AuthenticationProperties(new Dictionary<string, string?>
                                                          {
                                                              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                                                              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                                                          }));
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return Challenge(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The specified access token is bound to an account that no longer exists."
                    }));
            }

            var claims = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [Claims.Subject] = await userManager.GetUserIdAsync(user),
                [Claims.Email] = await userManager.GetEmailAsync(user) ?? string.Empty,
                [Claims.Name] = $"{user.FirstName} {user.MiddleNames} {user.LastName}",
                [Claims.FamilyName] = user.LastName ?? string.Empty,
                [Claims.GivenName] = user.FirstName ?? string.Empty,
                [Claims.MiddleName] = user.MiddleNames ?? string.Empty,
                [Claims.PreferredUsername] = await userManager.GetUserNameAsync(user) ?? string.Empty,
                [Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(user),
                [Claims.PhoneNumber] = await userManager.GetPhoneNumberAsync(user) ?? string.Empty,
                [Claims.PhoneNumberVerified] = await userManager.IsPhoneNumberConfirmedAsync(user),
                [Claims.Role] = await userManager.GetRolesAsync(user)
            };

            return Ok(claims);
        }

        private async Task<ClaimsIdentity> CreateClaimsIdentity(ApplicationUser user, OpenIddictRequest? request)
        {
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            // Get user data
            var userId = await userManager.GetUserIdAsync(user);
            var email = await userManager.GetEmailAsync(user);
            var username = await userManager.GetUserNameAsync(user);
            var roles = await userRoleService.GetUserRoleByClientId(user.Id, request?.ClientId);

            // Add standard claims
            identity.SetClaim(Claims.Subject, userId)
                    .SetClaim(Claims.Email, email)
                    .SetClaim(Claims.Name, $"{user.FirstName} {user.MiddleNames} {user.LastName}".Trim())
                    .SetClaim(Claims.PreferredUsername, username);

            // Add roles if any
            if (roles?.Data?.Any() == true)
            {
                identity.SetClaims(Claims.Role, [.. roles.Data?.Select(_ => _.RoleName).ToList() ?? []]);
            }

            // Set scopes and resources if applicable
            if (request is not null)
            {
                identity.SetScopes(request.GetScopes());

                var resources = await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync();
                identity.SetResources(resources);
            }

            // Configure destinations for each claim (identity + access token mapping)
            identity.SetDestinations(OAuthUtils.GetDestinations);

            return identity;
        }
    }
}