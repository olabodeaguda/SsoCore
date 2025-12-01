using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class CreateScopeRequestValidation : ValidationHelper<CreateScopeRequest>
    {
        public CreateScopeRequestValidation()
        {
            ValidateNotEmpty(x => x.Name, "Name");
            ValidateNotEmpty(x => x.DisplayName, "Display name");
            ValidateNotEmpty(x => x.Description, "Description");
        }
    }
}
