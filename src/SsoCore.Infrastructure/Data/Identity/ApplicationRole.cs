using Microsoft.AspNetCore.Identity;
using SsoCore.Application.DTOs;

namespace SsoCore.Infrastructure.Data.Identity
{
    public class ApplicationRole: IdentityRole<string>
    {
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

        public static ApplicationRole Create(string name, string? createdBy)
        {
            return new ApplicationRole
            {
                Name = name,
                NormalizedName = name.ToUpper(),
                CreatedBy = createdBy ?? "",
                CreatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                IsActive = true,
            };
        }

        public RoleDto ToModel()
        {
            return new RoleDto
            {
                Id = Id,
                Name = Name!,
                CreatedBy = CreatedBy,
                CreatedDate = CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public void Update(string roleName, string updatedBy)
        {
            Name = roleName;
            NormalizedName = roleName.ToUpper();
            LastUpdatedBy = updatedBy;
            LastUpdatedDate = DateTime.UtcNow;
        }

        internal void ActivateOrDeActivate(bool activate, string? updatedBy)
        {
            IsActive = activate;
            LastUpdatedBy = updatedBy;
            LastUpdatedDate = DateTime.UtcNow;
        }
    }
}
