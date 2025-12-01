namespace OnnexNotification
{
    public record EmailMessage
    {
        public string FullName { get; set;  } = string.Empty;
        public string Email { get; set;  } = string.Empty;
        public string Body { get; set;  } = string.Empty;
        public string Subject { get; set;  } = string.Empty;
        public string Title { get; set;  } = string.Empty;
        public string[] Attachements { get; set;  } = [];
        public string BannerMessage { get; set; } = string.Empty;
    }
}
