using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Handlers.Commands
{
    public class CreateClientRequest : IRequest<Result<ClientDto>>
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? DisplayName { get; set; }
        public string? ClientType { get; set; }
        public string? ConsentType { get; set; }
        public string? ApplicationType { get; set; }
        public List<string> Scopes { get; set; } = [];
        public List<string> GrantTypes { get; set; } = [];
        public List<string> ResponseTypes { get; set; } = [];
        public List<string> PostLogOutRedirectUri { get; set; } = [];
        public List<string> RedirectUri { get; set; } = [];
    }

    public class CreateClientHandler(IClientService clientService, IMapper mapper, ILogger<CreateClientHandler> logger) : IRequestHandler<CreateClientRequest, Result<ClientDto>>
    {
        private readonly IClientService _clientService = clientService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CreateClientHandler> _logger = logger;

        public async Task<Result<ClientDto>> Handle(CreateClientRequest model, CancellationToken cancellationToken)
        {
            try
            {
                var request = _mapper.Map<ClientDto>(model);

                var validateMetadata = _clientService.ValidateMetadata(request);
                if (!validateMetadata.IsSuccess) return Result<ClientDto>.Fail(validateMetadata.Error!);

                var client = await _clientService.GetByClientId(request.ClientId!);
                if (client.IsSuccess && client.Data != null) return Result<ClientDto>.Fail(ClientError.AlreadyExist());

                var result = await _clientService.CreateAsync(request);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to create client {@Client}", request);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                return Result<ClientDto>.Fail(ClientError.CreateFailed);
            }
        }
    }
}
