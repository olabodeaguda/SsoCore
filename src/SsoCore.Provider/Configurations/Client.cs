namespace SsoCore.Infrastructure.Models
{
    public class Client
    {
        public string? ClientSecret { get; set;  }
        public string? ClientId { get; set;  }
        public string? DisplayName { get; set;  }
        public string? ClientName { get; set;  }
        public string? ClientUri { get; set;  }
        public string? PostLogoutRedirectUri { get; set;  }
        public string? RedirectUri { get; set;  }
        public string? Scopes { get; set;  }
    }
}
