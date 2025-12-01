using MediatR;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Handlers.Commands
{
    public class CreateRoleRequest: IRequest<Result<RoleDto>>
    {
        public string RoleName { get; set; } = null!;
        public string? CreatedBy { get; set; }
    }

    public class CreateRoleHandler(IRoleService roleService, ILogger<CreateClientHandler> logger): IRequestHandler<CreateRoleRequest, Result<RoleDto>>
    {
        public async Task<Result<RoleDto>> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
        {
            var existing = await roleService.GetByName(request.RoleName);
            if(existing.IsSuccess)
            {
                logger.LogError("Role already exist {@Role}", request);
                return Result<RoleDto>.Fail(RoleError.AlreadyExist);
            }

            var result = await roleService.CreateAsync(new RoleDto
            {
                Name = request.RoleName,
                CreatedBy = request.CreatedBy
            });

            if (!result.IsSuccess)
            {
                logger.LogError("Failed to create role {@Role}", request);
                return Result<RoleDto>.Fail(existing.Error!);
            }

            return result;
        }
    }
}
