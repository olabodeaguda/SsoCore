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

namespace SsoCore.Application.Handlers.Queries
{
    public class GetAllScopesRequest : IRequest<Result<List<ScopeDto>>>
    {
    }

    public class GetAllScopesHandler : IRequestHandler<GetAllScopesRequest, Result<List<ScopeDto>>>
    {
        private readonly IScopeService _scopeService;
        private readonly ILogger<GetAllScopesHandler> _logger;
        public GetAllScopesHandler(IScopeService scopeService, ILogger<GetAllScopesHandler> logger)
        {
            _scopeService = scopeService;
            _logger = logger;
        }

        public async Task<Result<List<ScopeDto>>> Handle(GetAllScopesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _scopeService.GetAllScopes();
                return Result<List<ScopeDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all scopes");
                return Result<List<ScopeDto>>.Fail(ScopesError.GetAllScopeFailed());
            }
        }
    }
}
