using FluentValidation;
using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class CreateClientRequestValidation : ValidationHelper<CreateClientRequest>
    {
        public CreateClientRequestValidation()
        {
            ValidateNotEmpty(x => x.ClientId);
            ValidateNotEmpty(x => x.DisplayName);
            ValidateNotEmpty(x => x.ClientType);
            ValidateNotEmpty(x => x.ApplicationType);
            ValidateNotEmpty(x => x.ConsentType);
            ValidateListNotEmpty(_ => _.GrantTypes);
            RuleFor(x => x.ClientSecret)
                 .NotEmpty()
                 .When(x => !(x.ClientType ?? "").Equals("public"))
                 .WithMessage("ClientSecret is required when ClientType is not 'public'.");

        }
    }
}
