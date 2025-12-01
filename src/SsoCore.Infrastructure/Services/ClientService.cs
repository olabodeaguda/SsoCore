using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using System.Text.Json;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;
using SsoCore.Infrastructure.Data;
using SsoCore.Infrastructure.Helpers;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SsoCore.Infrastructure.Services
{
    public class ClientService(IOpenIddictApplicationManager openIddictApplicationManager,
        ILogger<ClientService> logger,
        IMapper mapper,
        ApplicationDbContext dbContext, IOpenIddictScopeManager openIddictScopeManager) : IClientService
    {
        public async Task<Result<ClientDto>> CreateAsync(ClientDto model)
        {
            try
            {
                var application = new OpenIddictApplicationDescriptor
                {
                    ClientId = model.ClientId,
                    DisplayName = model.DisplayName,
                    ClientType = model.ClientType,
                    ConsentType = model.ConsentType,
                    ApplicationType = model.ApplicationType,
                };

                if (!model.ClientType!.Equals(ClientTypes.Public, StringComparison.CurrentCultureIgnoreCase))
                    application.ClientSecret = model.ClientSecret;

                application.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
                var permissions = new List<string>(){
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Token,
                            Permissions.Endpoints.EndSession,
                            Permissions.Endpoints.Introspection,
                        };
                var grantTypes = model.GrantTypes.Select(_ => _.StartsWith(Permissions.Prefixes.GrantType)
                                                            ? _
                                                            : $"{Permissions.Prefixes.GrantType}{_}");

                permissions.AddRange(grantTypes);
                if (model.ResponseTypes.Count > 0)
                {
                    var responseTypes = model.ResponseTypes.Select(_ => _.StartsWith(Permissions.Prefixes.ResponseType)
                                                            ? _
                                                            : $"{Permissions.Prefixes.ResponseType}{_}");
                    permissions.AddRange(responseTypes);
                }

                if (model.Scopes.Count > 0)
                {
                    foreach (var item in model.Scopes)
                    {
                        var defaultScopes = GetMetadata().DefaultScopes;
                        if(defaultScopes.ContainsKey(item)) continue;

                        var scopeExisting = (OpenIddictEntityFrameworkCoreScope?)await openIddictScopeManager.FindByNameAsync(item).AsTask();
                        if (scopeExisting == null)
                        {
                            var scope = new OpenIddictScopeDescriptor
                            {
                                Name = item,
                                Resources = { model.ClientId! }
                            };
                            await openIddictScopeManager.CreateAsync(scope);
                        }
                        else
                        {
                            var scopeResources = await openIddictScopeManager.GetResourcesAsync(scopeExisting).AsTask();
                            if (!scopeResources.Contains(model.ClientId!))
                            {
                                scopeResources = scopeResources.Add(model.ClientId!);
                                scopeExisting!.Resources = JsonSerializer.Serialize(scopeResources);
                                await openIddictScopeManager.UpdateAsync(scopeExisting).AsTask();
                            }
                        }
                    }
                    var scopes = model.Scopes.Select(_ => _.StartsWith(Permissions.Prefixes.Scope)
                                                            ? _
                                                            : $"{Permissions.Prefixes.Scope}{_}");
                    permissions.AddRange(scopes);
                }
                if (model.PostLogOutRedirectUri.Count > 0)
                {
                    application.PostLogoutRedirectUris.UnionWith(model.PostLogOutRedirectUri.Select(_ => new Uri(_)));
                }
                if (model.RedirectUri.Count > 0)
                {
                    application.RedirectUris.UnionWith(model.RedirectUri.Select(_ => new Uri(_)));
                }

                application.Permissions.UnionWith(permissions);
                var result = await openIddictApplicationManager.CreateAsync(application);

                var descriptor = result as OpenIddictEntityFrameworkCoreApplication;

                return Result<ClientDto>.Success(mapper.Map<ClientDto>(descriptor));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create client {@Client}", model);
                return Result<ClientDto>.Fail(ClientError.CreateFailed);
            }
        }

        public async Task<Result<ClientDto>> GetByClientId(string clientId)
        {
            if ((await openIddictApplicationManager.FindByClientIdAsync(clientId)) is not OpenIddictEntityFrameworkCoreApplication entity) return Result<ClientDto>.Fail(ClientError.NotFound());

            return Result<ClientDto>.Success(mapper.Map<ClientDto>(entity));
        }

        public async Task<ClientDto[]> GetByClientId(List<string> resources)
        {
            var entities = await dbContext.Clients
                .Where(_ => resources.Any(x => x == _.ClientId!))
                .ToListAsync();

            var result = mapper.Map<ClientDto[]>(entities);

            return result;
        }

        public async Task<Pageable<ClientDto>> FilterAsync(string? search, int pageSize, int pageNumber)
        {
            var query = dbContext.Clients.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(_ =>
                               (_.ClientId != null && _.ClientId.Contains(search)) ||
                               (_.DisplayName != null && _.DisplayName.Contains(search)) ||
                               (_.ClientType != null && _.ClientType.Contains(search)));
            }

            var total = await query.CountAsync();
            var items = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return Pageable<ClientDto>.Create(mapper.Map<List<ClientDto>>(items), total, pageNumber, pageSize);
        }

        public ClientMetadataDto GetMetadata()
        {
            return new ClientMetadataDto()
            {
                ConsentTypes = OpenIdDictModelWrapperConstants.ConsentTypes(),
                ClientTypes = OpenIdDictModelWrapperConstants.ClientTypes(),
                ApplicationTypes = OpenIdDictModelWrapperConstants.ApplicationTypes(),
                GrantTypes = OpenIdDictModelWrapperConstants.GrantTypes(),
                ResponseTypes = OpenIdDictModelWrapperConstants.ResponseTypes(),
                DefaultScopes = OpenIdDictModelWrapperConstants.DefaultScopes().Concat(new Dictionary<string, string>
                {
                    { Scopes.OfflineAccess, "Offline Access" }
                }).ToDictionary(x => x.Key, x => x.Value)
            };
        }

        public Result ValidateMetadata(ClientDto model)
        {
            var clientMeta = GetMetadata();

            if (!clientMeta.ConsentTypes.Any(_ => _.Key.Equals(model.ConsentType, StringComparison.CurrentCultureIgnoreCase)))
                return Result.Fail(ClientError.ValidationError("Consent type is invalid"));

            if (!clientMeta.ClientTypes.Any(_ => _.Key.Equals(model.ClientType, StringComparison.CurrentCultureIgnoreCase)))
                return Result.Fail(ClientError.ValidationError("Client type is invalid"));

            if (!clientMeta.ApplicationTypes.Any(_ => _.Key.Equals(model.ApplicationType, StringComparison.CurrentCultureIgnoreCase)))
                return Result.Fail(ClientError.ValidationError("Application type is invalid"));

            if (model.GrantTypes?.Count > 0)
            {
                foreach (var grantType in model.GrantTypes)
                {
                    if (!clientMeta.GrantTypes.Any(_ => _.Key.Equals(grantType, StringComparison.CurrentCultureIgnoreCase)))
                        return Result.Fail(ClientError.ValidationError($"Grant type {grantType} is invalid"));
                }
            }
            return Result.Success("");
        }

        public async Task<Result<ClientDto>> UpdateAsync(ClientDto clientDTO)
        {
            try
            {
                var entity = await openIddictApplicationManager.FindByClientIdAsync(clientDTO.ClientId!);
                if (entity is not OpenIddictEntityFrameworkCoreApplication application) return Result<ClientDto>.Fail(ClientError.NotFound());

                var model = new OpenIddictApplicationDescriptor
                {
                    ClientId = clientDTO.ClientId
                };
                var permissions = string.IsNullOrEmpty(application.Permissions)
                    ? []
                    : JsonSerializer.Deserialize<string[]>(application.Permissions) ?? [];
                model.Permissions.UnionWith(permissions);

                var postLogoutRedirectUris = string.IsNullOrEmpty(application.PostLogoutRedirectUris)
                    ? []
                    : JsonSerializer.Deserialize<string[]>(application.PostLogoutRedirectUris)?.Select(_ => new Uri(_)) ?? [];
                model.PostLogoutRedirectUris.UnionWith(postLogoutRedirectUris);

                var redirectUris = string.IsNullOrEmpty(application.RedirectUris)
                        ? []
                        : JsonSerializer.Deserialize<string[]>(application.RedirectUris)?.Select(_ => new Uri(_)) ?? [];
                model.RedirectUris.UnionWith(redirectUris);

                if (!string.IsNullOrEmpty(clientDTO.ClientSecret))
                {
                    model.ClientSecret = clientDTO.ClientSecret;
                }
                if (!string.IsNullOrEmpty(clientDTO.DisplayName))
                {
                    model.DisplayName = clientDTO.DisplayName;
                }
                if (!string.IsNullOrEmpty(clientDTO.ClientType))
                {
                    model.ClientType = clientDTO.ClientType;
                }
                if (!string.IsNullOrEmpty(clientDTO.ConsentType))
                {
                    model.ConsentType = clientDTO.ConsentType;
                }
                if (!string.IsNullOrEmpty(clientDTO.ApplicationType))
                {
                    model.ApplicationType = clientDTO.ApplicationType;
                }
                if (clientDTO.GrantTypes.Count > 0)
                {
                    var grantTypesEntities = clientDTO.GrantTypes
                        .Select(_ => _.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType)
                                                                ? _
                                                                : $"{OpenIddictConstants.Permissions.Prefixes.GrantType}{_}");

                    var granTypes = model.Permissions!.Where(_ => _.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType));

                    var removedItems = granTypes.Except(grantTypesEntities);
                    if (removedItems.Any())
                    {
                        model.Permissions!.RemoveWhere(_ => removedItems.Any(x => x == _));
                    }
                    model.Permissions.UnionWith(grantTypesEntities);
                }

                if (clientDTO.ResponseTypes.Count > 0)
                {
                    var responseTypesEntities = clientDTO.ResponseTypes
                        .Select(_ => _.StartsWith(OpenIddictConstants.Permissions.Prefixes.ResponseType)
                                                                ? _
                                                                : $"{OpenIddictConstants.Permissions.Prefixes.ResponseType}{_}");

                    var responseTypes = model.Permissions!.Where(_ => _.StartsWith(OpenIddictConstants.Permissions.Prefixes.ResponseType));
                    var removedItems = responseTypes.Except(responseTypesEntities);
                    if (removedItems.Any())
                    {
                        model.Permissions.RemoveWhere(_ => removedItems.Any(x => x == _));
                    }
                    model.Permissions.UnionWith(responseTypesEntities);
                }

                if (clientDTO.Scopes.Count > 0)
                {
                    var scopesEntities = clientDTO.Scopes
                        .Select(_ => _.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope)
                                                                ? _
                                                                : $"{OpenIddictConstants.Permissions.Prefixes.Scope}{_}");

                    var scopes = model.Permissions!.Where(_ => _.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope));
                    var removedItems = scopes.Except(scopesEntities);
                    if (removedItems.Any())
                    {
                        model.Permissions.RemoveWhere(_ => removedItems.Any(x => x == _));
                    }

                    model.Permissions.UnionWith(scopesEntities);
                }

                if (clientDTO.PostLogOutRedirectUri.Count > 0)
                {
                    var pLogoutRedirectUriEntities = model.PostLogoutRedirectUris.Except(clientDTO.PostLogOutRedirectUri.Select(_ => new Uri(_)).ToHashSet());
                    if (pLogoutRedirectUriEntities.Any())
                    {
                        model.PostLogoutRedirectUris.RemoveWhere(_ => pLogoutRedirectUriEntities.Any(x => x == _));
                    }
                    model.PostLogoutRedirectUris.UnionWith(clientDTO.PostLogOutRedirectUri.Select(_ => new Uri(_)).ToHashSet());
                }
                if (clientDTO.RedirectUri.Count > 0)
                {
                    var redirectUriEntities = model.RedirectUris.Except(clientDTO.RedirectUri.Select(_ => new Uri(_)).ToHashSet());
                    if (redirectUriEntities.Any())
                    {
                        model.RedirectUris.RemoveWhere(_ => redirectUriEntities.Any(x => x == _));
                    }
                    model.RedirectUris.UnionWith(clientDTO.RedirectUri.Select(_ => new Uri(_)).ToHashSet());
                }

                await openIddictApplicationManager.UpdateAsync(application, model);

                var result = await openIddictApplicationManager.FindByClientIdAsync(clientDTO.ClientId!);

                var descriptor = result as OpenIddictEntityFrameworkCoreApplication;

                return Result<ClientDto>.Success(mapper.Map<ClientDto>(descriptor));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update client {@Client}", clientDTO);
                return Result<ClientDto>.Fail(ClientError.UpdateFailed);
            }
        }

        public async Task<Result<ClientDto>> UpdateSecretAsync(string clientId, string secret)
        {
            try
            {
                var entity = await openIddictApplicationManager.FindByClientIdAsync(clientId);
                if (entity is not OpenIddictEntityFrameworkCoreApplication application) return Result<ClientDto>.Fail(ClientError.NotFound());
                var model = new OpenIddictApplicationDescriptor
                {
                    ClientId = clientId,
                    ClientSecret = secret
                };
                await openIddictApplicationManager.UpdateAsync(application, secret);
                var result = await openIddictApplicationManager.FindByClientIdAsync(clientId);
                var descriptor = result as OpenIddictEntityFrameworkCoreApplication;
                return Result<ClientDto>.Success(mapper.Map<ClientDto>(descriptor));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update client secret {@Client}", clientId);
                return Result<ClientDto>.Fail(ClientError.UpdateFailed);
            }
        }
        
        public async Task<(bool isValid, ClientDto? client)> ValidateClientAndReturnUrl(string clientId, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(returnUrl))
            {
                return (false, null);
            }

            var clientResult = await GetByClientId(clientId);
            if (!clientResult.IsSuccess)
            {
                return (false, null);
            }

            var isValidReturnUrl = clientResult.Data != null && 
                                   (clientResult.Data.RedirectUri.Contains(returnUrl) || 
                                    clientResult.Data.PostLogOutRedirectUri.Contains(returnUrl));
    
            return (isValidReturnUrl, clientResult.Data);
        }
    }
}
