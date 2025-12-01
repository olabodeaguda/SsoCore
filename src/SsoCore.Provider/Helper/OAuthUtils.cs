using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;

namespace SsoCore.Provider.Helper
{
    public class OAuthUtils
    {
        public static IDictionary<string, StringValues> ParseOAuthParameters(HttpContext httpContext, List<string>? excluding = null)
        {
            excluding ??= [];

            var parameters = httpContext.Request.HasFormContentType
                ? httpContext.Request.Form
                    .Where(v => !excluding.Contains(v.Key))
                    .ToDictionary(v => v.Key, v => v.Value)
                : httpContext.Request.Query
                    .Where(v => !excluding.Contains(v.Key))
                    .ToDictionary(v => v.Key, v => v.Value);

            return parameters;
        }

        public static string BuildRedirectUrl(HttpRequest request, IDictionary<string, StringValues> oAuthParameters)
        {
            var url = request.PathBase + request.Path + QueryString.Create(oAuthParameters);
            return url;
        }

        public static bool IsAuthenticated(AuthenticateResult authenticateResult, OpenIddictRequest request)
        {
            if (!authenticateResult.Succeeded)
            {
                return false;
            }

            if (request.MaxAge.HasValue && authenticateResult.Properties != null)
            {
                var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);

                var expired = !authenticateResult.Properties.IssuedUtc.HasValue ||
                              DateTimeOffset.UtcNow - authenticateResult.Properties.IssuedUtc > maxAgeSeconds;
                if (expired)
                {
                    return false;
                }
            }

            return true;
        }

        public static IEnumerable<string> GetDestinations(Claim claim)
        {
            if (claim == null)
                yield break;

            var principal = claim.Subject;

            switch (claim.Type)
            {
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (principal?.HasScope(Scopes.Profile) == true)
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.PreferredUsername:
                    yield return Destinations.AccessToken;

                    if (principal?.HasScope(Scopes.Profile) == true)
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (principal?.HasScope(Scopes.Email) == true)
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (principal?.HasScope(Scopes.Roles) == true)
                        yield return Destinations.IdentityToken;

                    yield break;

                case "AspNet.Identity.SecurityStamp":
                    yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }

    }
}
