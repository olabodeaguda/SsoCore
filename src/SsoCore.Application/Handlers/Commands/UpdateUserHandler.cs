using MediatR;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Commands
{
    public class UpdateUserRequest: IRequest<Result<UserDto>>
    {
        public string? Id { get; set;  }
        public string? LastName { get; set;  }
        public string? FirstName { get; set;  }
        public string? MiddleNames { get; set;  }
        public string? UpdatedBy { get; set; }
    }

    public class UpdateUserHandler(IUserService userService, ILogger<UpdateUserHandler> logger) : IRequestHandler<UpdateUserRequest, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling UpdateUserRequest for user with ID: {Id}", request.Id);
            var result = await userService.UpdateUser(request.Id, request.FirstName, request.LastName, request.MiddleNames, request.UpdatedBy!);

            return result;
        }
    }
}
