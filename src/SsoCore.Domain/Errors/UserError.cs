using SsoCore.Domain.Common;

namespace SsoCore.Domain.Errors
{
    public class UserError
    {
        public static Error AlreadyExist = new("USER_ALREADY_EXIST", "User already exist");

        public static Error CreateFailed = new("USER_CREATE_FAILED", "User creation failed");

        public static Error NotFound = new("USER_NOT_FOUND", "User not found");

        public static Error UpdateFailed = new("USER_UPDATE_FAILED", "User update failed");

        public static Error DeActivateFailed = new("USER_DEACTIVATE_FAILED", "User deactivation failed");

        public static Error ActivateFailed = new("USER_ACTIVATE_FAILED", "User activation failed");
    }
}
