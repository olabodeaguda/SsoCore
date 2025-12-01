using MediatR;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Commands
{
    public class DeActivateUserRequest : IRequest<Result>
    {
        public string? Id { get; set;  }
        public string? UpdatedBy { get; set; }
    }

    public class DeActivateUserHandler: IRequestHandler<DeActivateUserRequest, Result>
    {
        private readonly IUserService _userService;
        public DeActivateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result> Handle(DeActivateUserRequest request, CancellationToken cancellationToken)
        {
            return await _userService.DeActivateUser(request.Id, request.UpdatedBy!);
        }
    }
}
