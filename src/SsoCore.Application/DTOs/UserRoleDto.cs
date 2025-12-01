namespace SsoCore.Application.DTOs
{
    public class UserRoleDto
    {
        public string RoleName { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;

        public string ClientId { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public string CreatedDate { get; set; } = null!;
        public string? UpdatedDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
