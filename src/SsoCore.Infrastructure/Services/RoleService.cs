using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;
using SsoCore.Infrastructure.Data.Identity;

namespace SsoCore.Infrastructure.Services
{
    public class RoleService(RoleManager<ApplicationRole> roleManager, ILogger<RoleService> logger) : IRoleService
    {
        public async Task<Result<RoleDto>> CreateAsync(RoleDto roleDTO)
        {
            try
            {
                var result = await roleManager.CreateAsync(ApplicationRole.Create(roleDTO.Name, roleDTO.CreatedBy));
                if (result.Succeeded)
                {
                    logger.LogInformation("Role {RoleName} created successfully", roleDTO.Name);
                    return await GetByName(roleDTO.Name);
                }

                return Result<RoleDto>.Fail(RoleError.CreateRoleFailed(string.Join(",", result.Errors.Select(e => e.Description).ToList())));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating role {RoleName}", roleDTO.Name);
                return Result<RoleDto>.Fail(RoleError.CreateRoleFailed());
            }
        }

        public async Task<Result<RoleDto>> GetByName(string roleName)
        {
            try
            {
                var role = await roleManager.FindByNameAsync(roleName.ToUpper());
                if (role == null)
                {
                    logger.LogError("Role {RoleName} not found", roleName);
                    return Result<RoleDto>.Fail(RoleError.RoleNotFound($"Role {roleName} not found"));
                }
                return Result<RoleDto>.Success(role.ToModel());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting role {RoleName}", roleName);
                return Result<RoleDto>.Fail(RoleError.GetRoleFailed);
            }
        }

        public async Task<Result<RoleDto>> ActivateOrDeactivate(string roleId, string? updatedBy, bool activate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                {
                    logger.LogWarning("Invalid role ID provided for activation/deactivation.");
                    return Result<RoleDto>.Fail(RoleError.InvalidRoleId("Role ID is required"));
                }

                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    logger.LogError("Role {RoleId} not found", roleId);
                    return Result<RoleDto>.Fail(RoleError.RoleNotFound($"Role {roleId} not found"));
                }

                role.ActivateOrDeActivate(activate, updatedBy);

                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Role {RoleId} {Status} successfully", roleId, activate ? "activated" : "deactivated");
                    return Result<RoleDto>.Success(role.ToModel());
                }

                logger.LogWarning("Failed to {Status} role {RoleId}. Errors: {Errors}",
                    activate ? "activate" : "deactivate",
                    roleId,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

                return Result<RoleDto>.Fail(RoleError.ActivateORDeactivatedFailed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while trying to {Status} role {RoleId}", activate ? "activate" : "deactivate", roleId);
                return Result<RoleDto>.Fail(RoleError.ActivateORDeactivatedFailed);
            }
        }

        public async Task<Pageable<RoleDto>> Filter(string? query, int page, int pageSize)
        {
            try
            {
                var roles = roleManager.Roles.AsQueryable();
                if (!string.IsNullOrEmpty(query))
                {
                    roles = roles.Where(r => r.Name!.Contains(query));
                }
                var totalItems = await roles.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var data = await roles
                    .OrderBy(r => r.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => r.ToModel())
                    .ToListAsync();
                return Pageable<RoleDto>.Create(data, totalItems, page, pageSize);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error filtering roles");
                return Pageable<RoleDto>.Empty;
            }
        }

        public async Task<Result<RoleDto>> UpdateAsync(string Id, string roleName, string updatedBy)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(Id);
                if (role == null)
                {
                    logger.LogError("Role {RoleId} not found", Id);
                    return Result<RoleDto>.Fail(RoleError.RoleNotFound($"Role {Id} not found"));
                }
                role.Update(roleName, updatedBy);

                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Role {RoleId} updated successfully", Id);
                    return Result<RoleDto>.Success(role.ToModel());
                }
                logger.LogWarning("Failed to update role {RoleId}. Errors: {Errors}",
                    Id,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return Result<RoleDto>.Fail(RoleError.CreateRoleFailed(string.Join(",", result.Errors.Select(e => e.Description).ToList())));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating role {RoleId}", Id);
                return Result<RoleDto>.Fail(RoleError.CreateRoleFailed());
            }
        }

        public async Task<Result<RoleDto>> GetById(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                logger.LogError("Role {RoleId} not found", Id);
                return Result<RoleDto>.Fail(RoleError.RoleNotFound($"Role {Id} not found"));
            }

            return Result<RoleDto>.Success(role.ToModel());
        }
    }
}
