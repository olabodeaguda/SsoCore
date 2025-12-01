namespace SsoCore.Application.DTOs
{
    public class ClientDto
    {
        public string? Id { get; set; }
        public string? ClientId { get; set;  }
        public string? ClientSecret { get; set;  }
        public string? DisplayName { get; set;  }
        public string? ClientType { get; set;  }
        public string? ConsentType { get; set;  }
        public string? ApplicationType { get; set;  }
        public List<string> GrantTypes { get; set;  } = [];
        public List<string> ResponseTypes { get; set;  } = [];
        public List<string> Scopes { get; set;  } = [];
        public List<string> PostLogOutRedirectUri { get; set;  } = [];
        public List<string> RedirectUri { get; set;  } = [];
    }
}
