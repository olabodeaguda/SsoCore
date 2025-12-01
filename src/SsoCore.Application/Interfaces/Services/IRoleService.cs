using SsoCore.Application.DTOs;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<Result<RoleDto>> CreateAsync(RoleDto roleDTO);
        Task<Result<RoleDto>> GetByName(string roleCode);
        Task<Result<RoleDto>> ActivateOrDeactivate(string roleId, string? updatedBy, bool activate);
        Task<Pageable<RoleDto>> Filter(string? query, int page, int pageSize);
        Task<Result<RoleDto>> UpdateAsync(string Id, string roleName, string updatedBy);
        Task<Result<RoleDto>> GetById(string Id);
    }
}
