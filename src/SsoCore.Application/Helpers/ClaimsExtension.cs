using System.Security.Claims;

namespace SsoCore.Application.Helpers
{
    public static class ClaimsExtension
    {
        public static string GetUserId(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        public static string GetName(this ClaimsPrincipal user) => user?.FindFirst(AppConstants.ClaimTypeName)?.Value ?? string.Empty;
    }
}
