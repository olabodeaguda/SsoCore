using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class UpdateUserRequestValidation : ValidationHelper<UpdateUserRequest>
    {
        public UpdateUserRequestValidation()
        {
            ValidateNotEmpty(x => x.LastName, "LastName");
            ValidateNotEmpty(x => x.FirstName, "FirstName");
            ValidateNotEmpty(x => x.Id, "Id");
        }
    }
}
