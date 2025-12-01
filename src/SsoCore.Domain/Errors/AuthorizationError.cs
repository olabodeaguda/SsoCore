using System.Net;
using SsoCore.Domain.Common;

namespace SsoCore.Domain.Errors
{
    public class AuthorizationError
    {
        public static Error Forbidden = new Error(
            "Authorization.Forbidden", "You do not have permission to access this resource.", (int)HttpStatusCode.Forbidden);

        public static Error AuthorizationFailed() => new Error(
            "Authorization.Failed", "Authorization failed", (int)HttpStatusCode.Unauthorized);

        public static Error AuthenticationFailed = new Error(
            "Authentication.failed", "Authentication failed. Please check your credentials.", (int)HttpStatusCode.Unauthorized);

        public static Error AuthenticationRequired = new Error(
            "Authentication.unauthorized", "Access denied. Please authenticate.", (int)HttpStatusCode.Unauthorized);
    }
}
