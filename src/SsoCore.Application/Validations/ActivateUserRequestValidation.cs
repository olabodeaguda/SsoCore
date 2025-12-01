using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class ActivateUserRequestValidation: ValidationHelper<ActivateUserRequest>
    {
        public ActivateUserRequestValidation()
        {
            ValidateNotEmpty(x => x.Id, "Id");
        }
    }
}
