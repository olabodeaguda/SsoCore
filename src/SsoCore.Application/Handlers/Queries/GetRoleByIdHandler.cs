using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Queries
{
    public class GetRoleByIdRequest: IRequest<Result<RoleDto>>
    {
        public string Id { get; set; } = null!;
    }

    public class GetRoleByIdHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdRequest, Result<RoleDto>>
    {
        public async Task<Result<RoleDto>> Handle(GetRoleByIdRequest request, CancellationToken cancellationToken)
        {
            return await roleService.GetById(request.Id);
        }
    }
}
