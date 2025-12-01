using FluentValidation;
using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    class CreateUserRequestValidation: ValidationHelper<CreateUserRequest>
    {
        public CreateUserRequestValidation()
        {

            ValidateNotEmpty(x => x.LastName, "LastName");
            ValidateNotEmpty(x => x.Email, "Email");
            ValidateNotEmpty(x => x.FirstName, "FirstName");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email");
        }
    }
}
