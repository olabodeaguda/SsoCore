using SsoCore.Domain.Common;

namespace SsoCore.Domain.Errors
{
    public class RoleError
    {
        public static Error GetRoleFailed => new("GetRoleFailed", "Failed to get role", 500);

        public static Error ActivateORDeactivatedFailed => new("ActivateORDeactivatedFailed", "Failed to activate or deactivate role", 500);

        public static Error AlreadyExist => new("AlreadyExist", "Role already exists", 400);

        public static Error CreateRoleFailed(string? message = null) => new("CreateRoleFailed", message?? "Create role failed", 500);

        public static Error InvalidRequest(string? message = null) => new("InvalidRequest", message ?? "Invalid request", 400);

        public static Error InvalidRoleId(string? message) => new("InvalidRoleId", message ?? "Invalid role ID", 400);

        public static Error RoleNotFound(string? message = null) => new("RoleNotFound", message ?? "Role not found", 404);
    }
}
