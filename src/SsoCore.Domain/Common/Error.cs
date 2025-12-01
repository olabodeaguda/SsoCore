using System.Net;

namespace SsoCore.Domain.Common
{
    public sealed record Error(string Code, string Description, int statusCode = (int)HttpStatusCode.BadRequest)
    {
        public static readonly Error None = new(string.Empty, string.Empty, (int)HttpStatusCode.BadRequest);
    }
}
