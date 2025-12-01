namespace SsoCore.Infrastructure.Configurations
{
    public class RabbitMQSettings
    {
        public string HostName { get; set;  } = string.Empty;
        public string UserName { get; set;  } = string.Empty;
        public string Password { get; set;  } = string.Empty;
        public string VHost { get; set;  } = string.Empty;
        public string EmailQueue { get; set;  } = string.Empty;
    }
}
