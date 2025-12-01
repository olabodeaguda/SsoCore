using System.Net;
using SsoCore.Domain.Common;

namespace SsoCore.Domain.Errors
{
    public class ClientError
    {
        public static readonly Error CreateFailed = new("CLIENT_CREATE_FAILED", "Client creation failed");

        public static readonly Error UpdateFailed = new("UPDATED_FAILED", "Client update failed");

        public static Error AlreadyExist() => new("CLIENT_ALREADY_EXIST", "Client already exist");

        public static Error NotFound() => new("CLIENT_NOT_FOUND", "Client not found", (int)HttpStatusCode.NotFound);

        public static Error ValidationError(string message) => new("CLIENT_VALIDATION_ERROR", message ?? "Client validation error");
    }
}
