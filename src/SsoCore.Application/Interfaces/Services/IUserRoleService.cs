using Microsoft.Extensions.Primitives;
using SsoCore.Application.DTOs;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Interfaces.Services
{
    public interface IUserRoleService
    {
        Task<Result<UserRoleDto>> ActivateOrDeactivateUserRoleAsync(string userId, string roleId,string clientId, bool shouldActive, string createdBy);
        Task<Result<UserRoleDto>> AssignRoleToUserAsync(string v1, string v2, string v3, string v4);
        Task<Result<UserRoleDto>> GetUserRoleAsync(string userId, string roleId);
        Task<Result<List<UserRoleDto>>> GetUserRoleAsync(string userId);
        Task<Result<List<UserRoleDto>>> GetUserRoleByClientId(string userId, string? clientId);
    }
}
