using MassTransit;
using Microsoft.Extensions.Logging;
using OnnexNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Configurations;

namespace SsoCore.Infrastructure.Services
{
    public class EmailService: IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly RabbitMQSettings _rabbitMqSettings;
        public EmailService(ILogger<EmailService> logger,
            ISendEndpointProvider sendEndpointProvider, RabbitMQSettings rabbitMqSettings)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _rabbitMqSettings = rabbitMqSettings;
        }

        public async Task SendAsync(EmailMessage model)
        {
            try
            {
                if (string.IsNullOrEmpty(_rabbitMqSettings.EmailQueue))
                {
                    _logger.LogInformation("RabbitMq queue name is not set");
                    return;
                }

                Uri uri = new Uri($"exchange:{_rabbitMqSettings.EmailQueue}");
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

                if (endpoint == null)
                {
                    _logger.LogInformation($"Message broker end point {uri.ToString()} could not be established");
                    return;
                }

                await endpoint.Send(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
            }
        }
    }
}
