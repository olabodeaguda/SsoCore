using MediatR;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Handlers.Commands
{
    public class ActivateOrDeactivateUserRoleRequest: IRequest<Result<UserRoleDto>>
    {
        public string? ClientId { get; set; }
        public string? UserId { get; set; }
        public string? RoleId { get; set; }
        public string? CreatedBy { get; set; }
        public bool ShouldActive { get; set; }
    }

    public class ActivateOrDeactivateUserRoleHandler(IRoleService roleService, 
        IUserService userService,
        IUserRoleService userRoleService,
        ILogger<ActivateOrDeactivateUserRoleHandler> logger,
        IClientService clientService) : IRequestHandler<ActivateOrDeactivateUserRoleRequest, Result<UserRoleDto>>
    {
        public async Task<Result<UserRoleDto>> Handle(ActivateOrDeactivateUserRoleRequest request, CancellationToken cancellationToken)
        {
            var role = await roleService.GetById(request.RoleId!);
            if(role == null)
            {
                logger.LogError("Role not found");
                return Result<UserRoleDto>.Fail(RoleError.RoleNotFound());
            }
            var user = await userService.GetUserByIdAsync(request.UserId!);
            if(user == null)
            {
                logger.LogError("User not found");
                return Result<UserRoleDto>.Fail(UserError.NotFound);
            }


            var client = await clientService.GetByClientId(request.ClientId!);
            if (client == null)
            {
                logger.LogError("Client not found");
                return Result<UserRoleDto>.Fail(ClientError.NotFound());
            }
            var userRole = await userRoleService.GetUserRoleAsync(request.UserId!, request.RoleId!);
            if(userRole == null)
            {
                logger.LogError("User role not found");
                return Result<UserRoleDto>.Fail(UserRoleError.NotFound);
            }

            return await userRoleService.ActivateOrDeactivateUserRoleAsync(request.UserId!,
                request.RoleId!, 
                client.Data!.Id!, 
                request.ShouldActive,
                request.CreatedBy!);            
        }
    }
}
