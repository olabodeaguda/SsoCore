using AutoMapper;
using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Handlers.Commands
{
    public class CreateScopeRequest : IRequest<Result<ScopeDto>>
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public List<string> Resources { get; set; } = new();
        public List<string> Properties { get; set; } = new();
    }

    public class CreateScopeHandler : IRequestHandler<CreateScopeRequest, Result<ScopeDto>>
    {
        private readonly IScopeService _scopeService;
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;
        public CreateScopeHandler(IScopeService scopeService, IClientService clientService, IMapper mapper)
        {
            _scopeService = scopeService;
            _clientService = clientService;
            _mapper = mapper;
        }

        public async Task<Result<ScopeDto>> Handle(CreateScopeRequest request, CancellationToken cancellationToken)
        {
            var scope = await _scopeService.GetByName(request.Name!);
            if (scope != null) return Result<ScopeDto>.Fail(ScopesError.AlreadyExist());

            return await _scopeService.CreateScope(_mapper.Map<ScopeDto>(request));
        }

        public async Task<Result> ValidateRequest(CreateScopeRequest request)
        {
            if (request.Resources.Count > 0)
            {
                var clients = await _clientService.GetByClientId(request.Resources);
                if (clients.Length != request.Resources.Count)
                {
                    var invalidResources = request.Resources.Except(clients.Select(x => x.ClientId)).ToList();
                    return Result.Fail(ClientError.ValidationError($"{String.Join(',', invalidResources)} not found."));
                }
            }
            return Result.Success("Validation success");
        }
    }
}
