using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Queries
{
    public class GetRolesRequest:IRequest<Pageable<RoleDto>>
    {
        public string? Query { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetRolesHandler(IRoleService roleService): IRequestHandler<GetRolesRequest, Pageable<RoleDto>>
    {
        public async Task<Pageable<RoleDto>> Handle(GetRolesRequest request, CancellationToken cancellationToken)
        {
            return await roleService.Filter(request.Query, request.PageNumber, request.PageSize);
        }
    }
}
