using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Validations
{
    public class UpdateClientSecretRequestValidation: ValidationHelper<UpdateClientSecretRequest>
    {
        public UpdateClientSecretRequestValidation()
        {
            ValidateNotEmpty(x => x.ClientId, "ClientId");
            ValidateNotEmpty(x => x.ClientSecret, "ClientSecret");
        }
    }
}
