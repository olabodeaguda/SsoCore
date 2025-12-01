namespace SsoCore.Application.DTOs
{
    public class RoleDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
    }
}
