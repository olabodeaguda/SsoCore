using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Repositories;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;
using SsoCore.Infrastructure.Data.Identity;

namespace SsoCore.Infrastructure.Services
{
    public class UserRoleService(IRepository<ApplicationUserRole> userRoleRepository, ILogger<UserRoleService> logger) : IUserRoleService
    {
        public async Task<Result<UserRoleDto>> ActivateOrDeactivateUserRoleAsync(string userId, string roleId, string clientId, bool shouldActive, string createdBy)
        {
            var userRole = await userRoleRepository
                        .GetSingleAsync(x =>
                        x.Where(_ => _.UserId == userId && _.RoleId == roleId && _.ClientId == clientId), ["Client", "User", "Role"]);
            if (userRole == null)
            {
                logger.LogError("User role not found");
                return Result<UserRoleDto>.Fail(UserRoleError.NotFound);
            }

            userRole.ActivateOrDeactivate(shouldActive, createdBy);

            var result = await userRoleRepository.UpdateAsync(userRole);
            if (!result)
            {
                logger.LogError("Error updating user role");
                return Result<UserRoleDto>.Fail(UserRoleError.UpdateFailed);
            }

            return await GetUserRoleAsync(userId, roleId);
        }

        public async Task<Result<UserRoleDto>> AssignRoleToUserAsync(string userId, string roleId, string createdBy, string clientId)
        {
            var entity = ApplicationUserRole.Create(userId, roleId, clientId, createdBy);
            
            var result = await userRoleRepository.AddAsync(entity);
            if (result == null)
            {
                logger.LogError("Error assigning role to user");
                return Result<UserRoleDto>.Fail(UserRoleError.CreateFailed);
            }

            return Result<UserRoleDto>.Success(result.ToModel());
        }

        public async Task<Result<UserRoleDto>> GetUserRoleAsync(string userId, string roleId)
        {
            try
            {
                var userRole = await userRoleRepository
                        .GetSingleAsync(x => x.Where(_ => _.UserId == userId && _.RoleId == roleId), 
                            d=>d.Role, d=>d.User, d=>d.Client);
                if (userRole == null)
                {
                    logger.LogError("User role not found");
                    return Result<UserRoleDto>.Fail(UserRoleError.NotFound);
                }

                return Result<UserRoleDto>.Success(userRole.ToModel());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user role");

                return Result<UserRoleDto>.Fail(UserRoleError.NotFound);
            }
        }

        public async Task<Result<List<UserRoleDto>>> GetUserRoleAsync(string userId)
        {
            var entities = await userRoleRepository
                .GetAllAsync(x => x.Where(_ => _.UserId == userId), ["Client", "User", "Role"]);

            var result = entities
                .Select(x => x.ToModel())
                .ToList();

            return Result<List<UserRoleDto>>.Success(result);
        }

        public async Task<Result<List<UserRoleDto>>> GetUserRoleByClientId(string userId, string? clientId)
        {
            try
            {
                var entities = await userRoleRepository
                       .GetAllAsync(x => x.Where(_ => _.UserId == userId && _.Client.ClientId!.ToLower() == clientId!.ToLower()), ["Client", "User", "Role"]);

                var result = entities
                    .Select(x => x.ToModel())
                    .ToList();

                return Result<List<UserRoleDto>>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user role by client id");
                return Result<List<UserRoleDto>>.Fail(UserRoleError.NotFound);
            }
        }
    }
}
