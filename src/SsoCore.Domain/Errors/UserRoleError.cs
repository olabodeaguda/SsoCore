using SsoCore.Domain.Common;

namespace SsoCore.Domain.Errors
{
    public class UserRoleError
    {
        public static Error NotFound => new("UserRoleNotFound", "User role not found", 404);

        public static Error AlreadyExist => new("UserRoleAlreadyExist", "User role already exist", 400);

        public static Error CreateFailed => new("UserRoleCreateFailed", "User role assignment failed", 500);

        public static Error UpdateFailed => new("UserRoleUpdateFailed", "User role update failed", 500);
    }
}
