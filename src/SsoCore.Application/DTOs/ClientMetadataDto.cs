namespace SsoCore.Application.DTOs
{
    public class ClientMetadataDto
    {
        public Dictionary<string, string> ConsentTypes { get; set; } = new();
        public Dictionary<string, string> ClientTypes { get; set; } = new();
        public Dictionary<string, string> ApplicationTypes { get; set; } = new();
        public Dictionary<string, string> GrantTypes { get; set; } = new();
        public Dictionary<string, string> ResponseTypes { get; set; } = new();
        public Dictionary<string, string> DefaultScopes { get; set; } = new();
    }
}
