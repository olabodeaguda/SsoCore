using AutoMapper;
using MediatR;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;

namespace SsoCore.Application.Handlers.Commands
{
    public class UpdateScopeRequest : IRequest<Result<ScopeDto>>
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public List<string> Resources { get; set; } = new();
    }

    public class UpdateScopeHandler: IRequestHandler<UpdateScopeRequest, Result<ScopeDto>>
    {
        private readonly IScopeService _scopeService;
        private readonly IMapper _mapper;
        public UpdateScopeHandler(IScopeService scopeService, IMapper mapper)
        {
            _scopeService = scopeService;
            _mapper = mapper;
        }

        public async Task<Result<ScopeDto>> Handle(UpdateScopeRequest request, CancellationToken cancellationToken)
        {
            var scope = await _scopeService.GetByName(request.Name!);
            if (scope == null)
                return Result<ScopeDto>.Fail(ScopesError.NotFound());

            return await _scopeService.UpdateScope(_mapper.Map<ScopeDto>(request));
        }
    }
}
