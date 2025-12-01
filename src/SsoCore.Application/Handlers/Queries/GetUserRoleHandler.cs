using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Queries
{
    public class GetUserRoleRequest: IRequest<Result<List<UserRoleDto>>>
    {
        public string UserId { get; set; } = null!;
    }

    public class GetUserRoleHandler(IUserRoleService userRoleService) : IRequestHandler<GetUserRoleRequest, Result<List<UserRoleDto>>>
    {
        public async Task<Result<List<UserRoleDto>>> Handle(GetUserRoleRequest request, CancellationToken cancellationToken)
        {
            
            return await userRoleService.GetUserRoleAsync(request.UserId);
        }
    }
}
