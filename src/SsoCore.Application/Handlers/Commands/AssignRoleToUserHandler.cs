using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Handlers.Commands
{
    public class AssignRoleToUserRequest: IRequest<Result<UserRoleDto>>
    {
        public string? RoleId { get; set; }
        public string? UserId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ClientId { get; set; }
    }

    public class AssignRoleToUserHandler(IRoleService roleService,
        IUserService userService,
        IUserRoleService userRoleService,
        ILogger<AssignRoleToUserHandler> logger,
        IClientService clientService) : IRequestHandler<AssignRoleToUserRequest, Result<UserRoleDto>>
    {
        public async Task<Result<UserRoleDto>> Handle(AssignRoleToUserRequest request, CancellationToken cancellationToken)
        {
            var role = await roleService.GetById(request.RoleId!);
            if (role == null)
            {
                logger.LogError("Role not found");
                return Result<UserRoleDto>.Fail(RoleError.RoleNotFound());
            }
            var user = await userService.GetUserByIdAsync(request.UserId!);
            if (user == null)
            {
                logger.LogError("User not found");
                return Result<UserRoleDto>.Fail(UserError.NotFound);
            }

            var client = await clientService.GetByClientId(request.ClientId!);
            if(client == null)
            {
                logger.LogError("Client not found");
                return Result<UserRoleDto>.Fail(ClientError.NotFound());
            }

            var userRole = await userRoleService.GetUserRoleAsync(request.UserId!, request.RoleId!);
            if (userRole.IsSuccess)
            {
                logger.LogError("User role already found");
                return Result<UserRoleDto>.Fail(UserRoleError.AlreadyExist);
            }

            return await userRoleService.AssignRoleToUserAsync(request.UserId!, request.RoleId!, request.CreatedBy!, client.Data!.Id!);
        }
    }
}
