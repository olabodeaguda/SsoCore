using OnnexNotification;

namespace SsoCore.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage model);
    }
}
