using FluentValidation;
using SsoCore.Application.Handlers.Queries;

namespace SsoCore.Application.Validations
{
    public class GetUserByIdRequestValidation: AbstractValidator<GetUserByIdRequest>
    {
        public GetUserByIdRequestValidation()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required");
        }
    }
}
