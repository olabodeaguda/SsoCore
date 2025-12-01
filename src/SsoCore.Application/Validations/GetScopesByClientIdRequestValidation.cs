using SsoCore.Application.Handlers.Queries;

namespace SsoCore.Application.Validations
{
    public class GetScopesByClientIdRequestValidation : ValidationHelper<GetScopesByClientIdRequest>
    {
        public GetScopesByClientIdRequestValidation()
        {
            ValidateNotEmpty(x => x.ClientId, "Client ID");
        }
    }
}
