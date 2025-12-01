using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class DeActivateUserRequestValidation: ValidationHelper<DeActivateUserRequest>
    {
        public DeActivateUserRequestValidation()
        {
            ValidateNotEmpty(x => x.Id, "Id");
        }
    }
}
