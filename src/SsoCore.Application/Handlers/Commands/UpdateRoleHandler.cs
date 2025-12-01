using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Commands
{
    public class UpdateRoleRequest: IRequest<Result<RoleDto>>
    {
        public string Id { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string? UpdatedBy { get; set; }
    }

    public class UpdateRoleHandler(IRoleService roleService) : IRequestHandler<UpdateRoleRequest, Result<RoleDto>>
    {
        public async Task<Result<RoleDto>> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            return await roleService.UpdateAsync(request.Id, request.RoleName, request.UpdatedBy!);
        }
    }
}
