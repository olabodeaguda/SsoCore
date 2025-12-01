using System.Net;
using SsoCore.Domain.Common;

namespace SsoCore.Domain.Errors
{
    public class ScopesError
    {
        public static Error AlreadyExist() => new Error("SCOPE_ALREADY_EXIST", "Scope already exist");

        public static Error CreateFailed() => new Error("CREATE_SCOPE_FAILED", "Failed to create scope");

        public static Error GetAllScopeFailed() => new Error("GET_ALL_SCOPE_FAILED", "Failed to get all scopes");

        public static Error NotFound() => new Error("SCOPE_NOT_FOUND", "Scope not found", (int)HttpStatusCode.NotFound);

        public static Error UpdateFailed() => new Error("UPDATE_SCOPE_FAILED", "Failed to update scope");
    }
}
