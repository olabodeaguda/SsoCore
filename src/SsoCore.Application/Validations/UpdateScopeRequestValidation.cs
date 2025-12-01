using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class UpdateScopeRequestValidation: ValidationHelper<UpdateScopeRequest>
    {
        public UpdateScopeRequestValidation()
        {
            ValidateNotEmpty(x => x.Name, "Name");
            ValidateNotEmpty(x => x.DisplayName, "Display name");
            ValidateNotEmpty(x => x.Description, "Description");
        }
    }
}
