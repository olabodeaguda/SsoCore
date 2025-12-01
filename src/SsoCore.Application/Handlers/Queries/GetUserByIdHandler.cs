using MediatR;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Queries
{
    public class GetUserByIdRequest : IRequest<Result<UserDto>>
    {
        public string? UserId { get; set; }
    }

    public class GetUserByIdHandler(IUserService userService, ILogger<GetUserByIdHandler> logger) : IRequestHandler<GetUserByIdRequest, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting user by id {UserId}", request.UserId);
            var user = await userService.GetUserByIdAsync(request.UserId!);

            return user;
        }
    }
}
