using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Commands
{
    public class ActivateUserRequest : IRequest<Result<UserDto>>
    {
        public string? Id { get; set;  }
        public string? UpdatedBy { get; set; }
    }

    public class ActivateUserHandler: IRequestHandler<ActivateUserRequest, Result<UserDto>>
    {
        private readonly IUserService _userService;
        public ActivateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<UserDto>> Handle(ActivateUserRequest request, CancellationToken cancellationToken)
        {
            return await _userService.ActivateUser(request.Id, request.UpdatedBy!);
        }
    }
}
