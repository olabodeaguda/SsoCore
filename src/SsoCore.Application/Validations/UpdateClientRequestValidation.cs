using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class UpdateClientRequestValidation: ValidationHelper<UpdateClientRequest>
    {
        public UpdateClientRequestValidation()
        {
            ValidateNotEmpty(x => x.ClientId, "Client ID");
            ValidateNotEmpty(x => x.DisplayName, "Display name");
            ValidateNotEmpty(x => x.ClientType, "Client type");
            ValidateNotEmpty(x => x.ApplicationType, "Application type");
            ValidateNotEmpty(x => x.ConsentType, "Consent type");
            ValidateListNotEmpty<string>(_ => _.GrantTypes, "Grant types");
            RuleFor(x => x.ClientSecret)
                 .NotEmpty()
                 .When(x => !x.ClientType!.Equals("public"))
                 .WithMessage("ClientSecret is required when ClientType is not 'public'.");
        }
    }
}
