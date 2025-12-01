using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Handlers.Commands
{
    public class UpdateClientRequest : IRequest<Result<ClientDto>>
    {
        public string? ClientId { get; set;  }
        public string? ClientSecret { get; set;  }
        public string? DisplayName { get; set;  }
        public string? ClientType { get; set;  }
        public string? ConsentType { get; set;  }
        public string? ApplicationType { get; set;  }
        public List<string>? GrantTypes { get; set;  }
        public List<string> ResponseTypes { get; set;  } = new();
        public List<string> Scopes { get; set;  } = new();
        public List<string> PostLogOutRedirectUri { get; set;  } = new();
        public List<string> RedirectUri { get; set;  } = new();
    }

    public class UpdateClientHandler : IRequestHandler<UpdateClientRequest, Result<ClientDto>>
    {
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateClientHandler> _logger;
        public UpdateClientHandler(IClientService clientService, IMapper mapper, ILogger<UpdateClientHandler> logger)
        {
            _clientService = clientService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Result<ClientDto>> Handle(UpdateClientRequest request, CancellationToken cancellationToken)
        {
            var client = await _clientService.GetByClientId(request.ClientId!);
            if (!client.IsSuccess || client.Data == null) return Result<ClientDto>.Fail(ClientError.NotFound());

            var result = await _clientService.UpdateAsync(_mapper.Map<ClientDto>(request));
            if (!result.IsSuccess)
                _logger.LogError("Failed to update client {@Client}", request);

            return result;
        }
    }
}
