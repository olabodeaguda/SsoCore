using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;

namespace SsoCore.Application.Handlers.Queries
{
    public class GetClientMetadataRequest : IRequest<ClientMetadataDto>
    {
    }

    public class GetClientMetadataHandler : IRequestHandler<GetClientMetadataRequest, ClientMetadataDto>
    {
        private readonly IClientService _clientService;
        public GetClientMetadataHandler(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<ClientMetadataDto> Handle(GetClientMetadataRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_clientService.GetMetadata());
        }
    }
}
