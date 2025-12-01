using System.Security.Claims;

namespace SsoCore.Application.Helpers
{
    public static class HelperExtension
    {
        public static string? UrlEncoded(this string src) => src == null ? src : System.Net.WebUtility.UrlEncode(src);
        public static string GenerateUniqueId = Guid.NewGuid().ToString("N");
    }
}
