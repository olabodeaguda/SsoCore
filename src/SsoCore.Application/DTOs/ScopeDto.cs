namespace SsoCore.Application.DTOs
{
    public class ScopeDto
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public List<string> Resources { get; set; } = new();
    }
}
