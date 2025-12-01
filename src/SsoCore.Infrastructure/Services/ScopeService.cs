using AutoMapper;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;
using SsoCore.Infrastructure.Helpers;

namespace SsoCore.Infrastructure.Services
{
    public class ScopeService(IOpenIddictScopeManager scopeManager, ILogger<ScopeService> logger, IMapper mapper) : IScopeService
    {
        private readonly IOpenIddictScopeManager _scopeManager = scopeManager;
        private readonly ILogger<ScopeService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<ScopeDto>> CreateScope(ScopeDto scopeDTO)
        {
            var entity = new OpenIddictScopeDescriptor
            {
                Name = scopeDTO.Name,
                DisplayName = scopeDTO.DisplayName,
                Description = scopeDTO.Description
            };
            entity.Resources.UnionWith(scopeDTO.Resources);

            var result = await _scopeManager.CreateAsync(entity);

            if (result == null)
            {
                _logger.LogError("Failed to create scope {@Scope}", scopeDTO);
                return Result<ScopeDto>.Fail(ScopesError.CreateFailed());
            }

            var scope = result as OpenIddictEntityFrameworkCoreScope;
            return Result<ScopeDto>.Success(_mapper.Map<ScopeDto>(scope));
        }

        public async Task<List<ScopeDto>> GetAllScopes()
        {
            var result = new List<ScopeDto>();
            try
            {
                var scopes = _scopeManager.ListAsync();
                var entities = new List<OpenIddictEntityFrameworkCoreScope>();
                await foreach (var item in scopes)
                {
                    if (item is OpenIddictEntityFrameworkCoreScope scope)
                        entities.Add(scope);
                }

                result = _mapper.Map<List<ScopeDto>>(entities);

                result.AddRange(OpenIdDictModelWrapperConstants.DefaultScopes().Select(_ => new ScopeDto() { Name = _.Key, DisplayName = _.Value }));

                return [.. result.OrderBy(_ => _.Name)];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all scopes");
                return [];
            }
        }

        public async Task<ScopeDto?> GetByName(string name)
        {
            var scope = await _scopeManager.FindByNameAsync(name);
            if (scope == null) return null;

            if (scope is OpenIddictEntityFrameworkCoreScope scopeEntity)
                return _mapper.Map<ScopeDto>(scopeEntity);

            return null;
        }

        public async Task<Result<ScopeDto>> UpdateScope(ScopeDto scopeDTO)
        {
            try
            {
                var existingEnity = await _scopeManager.FindByNameAsync(scopeDTO.Name!);
                if (existingEnity == null)
                {
                    _logger.LogError("Scope {@Scope} not found", scopeDTO);
                    return Result<ScopeDto>.Fail(ScopesError.NotFound());
                }
                var entity = new OpenIddictScopeDescriptor
                {
                    Name = scopeDTO.Name,
                    DisplayName = scopeDTO.DisplayName,
                    Description = scopeDTO.Description
                };
                entity.Resources.UnionWith(scopeDTO.Resources);

                await _scopeManager.UpdateAsync(existingEnity, entity);

                var scope = await _scopeManager.FindByNameAsync(scopeDTO.Name!);
                return Result<ScopeDto>.Success(_mapper.Map<ScopeDto>(scope));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update scope {@Scope}", scopeDTO);
                return Result<ScopeDto>.Fail(ScopesError.UpdateFailed());
            }
        }
    }
}
