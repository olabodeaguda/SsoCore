using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Commands
{
    public class UpdateClientSecretRequest : IRequest<Result<ClientDto>>
    {
        public string? ClientId { get; set;  }
        public string? ClientSecret { get; set;  }
    }

    public class UpdateClientSecretHandler : IRequestHandler<UpdateClientSecretRequest, Result<ClientDto>>
    {
        private readonly IClientService _clientService;

        public UpdateClientSecretHandler(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<Result<ClientDto>> Handle(UpdateClientSecretRequest request, CancellationToken cancellationToken)
        {
            var client = await _clientService.GetByClientId(request.ClientId!);
            if (!client.IsSuccess) return Result<ClientDto>.Fail(client.Error!);

            return await _clientService.UpdateSecretAsync(request.ClientId!, request.ClientSecret!);
        }
    }
}
