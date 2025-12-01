using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Queries
{
    public class FilterClientRequest : IRequest<Pageable<ClientDto>>
    {
        public string? Search { get; set;  }
        public int PageSize { get; set;  }
        public int PageNumber { get; set;  }
    }

    public class FilterClientHandler : IRequestHandler<FilterClientRequest, Pageable<ClientDto>>
    {
        private IClientService _clientService { get; }
        public FilterClientHandler(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<Pageable<ClientDto>> Handle(FilterClientRequest request, CancellationToken cancellationToken)
        {
            return await _clientService.FilterAsync(request.Search, request.PageSize, request.PageNumber);
        }
    }
}
