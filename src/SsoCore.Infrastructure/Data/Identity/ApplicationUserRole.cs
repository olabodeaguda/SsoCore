using Microsoft.AspNetCore.Identity;
using OpenIddict.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SsoCore.Application.DTOs;

namespace SsoCore.Infrastructure.Data.Identity
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public string ClientId { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public bool IsActive { get; set; }

        public OpenIddictEntityFrameworkCoreApplication Client { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        public ApplicationRole Role { get; set; } = null!;

        internal static ApplicationUserRole Create(string userId, string roleId, string clientId, string createdBy)
        {
            return new ApplicationUserRole
            {
                UserId = userId,
                RoleId = roleId,
                ClientId = clientId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };
        }

        internal UserRoleDto ToModel()
        {
            return new UserRoleDto
            {
                RoleName = Role?.Name ?? "",
                RoleId = Role?.Id ?? "",
                UserId = User.Id,
                UserName = User?.UserName ?? "",
                ClientId = ClientId,
                CreatedBy = CreatedBy,
                CreatedDate = CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedDate = UpdatedDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                LastUpdatedBy = LastUpdatedBy,
                IsActive = IsActive
            };
        }

        internal void ActivateOrDeactivate(bool shouldActive, string createdBy)
        {
            IsActive = shouldActive;
            LastUpdatedBy = createdBy;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}
