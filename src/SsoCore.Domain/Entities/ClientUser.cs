namespace SsoCore.Domain.Entities
{
    public class ClientUser
    {
        public string? ClientId { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
