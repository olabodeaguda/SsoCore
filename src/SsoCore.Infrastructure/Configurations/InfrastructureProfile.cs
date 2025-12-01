using AutoMapper;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using System.Text.Json;
using SsoCore.Application.DTOs;
using SsoCore.Application.Helpers;
using SsoCore.Infrastructure.Data.Identity;

namespace SsoCore.Infrastructure.Configurations
{
    public class InfrastructureProfile:Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(_ => _.UserName, src => src.MapFrom(x => x.Email))
                .ForMember(s=>s.Id, src=> src.MapFrom(x=> HelperExtension.GenerateUniqueId))
                .ForMember(s => s.CreatedAt, src => src.MapFrom(x => DateTime.UtcNow));

            CreateMap<OpenIddictApplicationDescriptor, ClientDto>()
                .ConvertUsing(_ => MapApplicationDescriptorToClient(_));
            CreateMap<OpenIddictEntityFrameworkCoreApplication, ClientDto>()
                .ConvertUsing(_ => MapApplicationDescriptorToClient(_));
            CreateMap<OpenIddictEntityFrameworkCoreScope, ScopeDto>()
                .ConvertUsing(_ => MapScopeToScopeDTO(_));
        }

        private static ScopeDto MapScopeToScopeDTO(OpenIddictEntityFrameworkCoreScope model)
        {
            var scope = new ScopeDto
            {
                Name = model.Name,
                Resources = !string.IsNullOrEmpty(model.Resources) ? [.. JsonSerializer.Deserialize<string[]>(model.Resources!)!] : [],
                DisplayName = model.DisplayName,
                Description = model.Description,
            };

            return scope;
        }

        private static ClientDto MapApplicationDescriptorToClient(OpenIddictEntityFrameworkCoreApplication model)
        {
            var permissions = model.Permissions == null || model.Permissions.Length < 1 ? [] : JsonSerializer.Deserialize<List<string>>(model.Permissions)!;

            return new ClientDto
            {
                Id = model.Id!.ToString(),
                ClientId = model.ClientId,
                ClientSecret = string.Empty,
                DisplayName = model.DisplayName,
                ClientType = model.ClientType,
                ConsentType = model.ConsentType,
                ApplicationType = model.ApplicationType,
                GrantTypes = [.. permissions.Where(_ => _.StartsWith("gt"))],
                ResponseTypes = [.. permissions.Where(_ => _.StartsWith("rst"))],
                Scopes = [.. permissions.Where(_ => _.StartsWith("scp"))],
                PostLogOutRedirectUri = string.IsNullOrEmpty(model.PostLogoutRedirectUris) ? [] : [.. JsonSerializer.Deserialize<List<string>>(model.PostLogoutRedirectUris!)!.Select(_ => new Uri(_).ToString())],
                RedirectUri = string.IsNullOrEmpty(model.RedirectUris) ? [] : [.. JsonSerializer.Deserialize<List<string>>(model.RedirectUris!)!.Select(_ => new Uri(_).ToString())],
            };
        }

        private static ClientDto MapApplicationDescriptorToClient(OpenIddictApplicationDescriptor model)
        {
            return new ClientDto
            {
                ClientId = model.ClientId,
                ClientSecret = string.Empty,
                DisplayName = model.DisplayName,
                ClientType = model.ClientType,
                ConsentType = model.ConsentType,
                ApplicationType = model.ApplicationType,
                GrantTypes = [.. model.Permissions.Where(_ => _.StartsWith("gt"))],
                ResponseTypes = [.. model.Permissions.Where(_ => _.StartsWith("rst"))],
                Scopes = [.. model.Permissions.Where(_ => _.StartsWith("scp"))],
                PostLogOutRedirectUri = [.. model.PostLogoutRedirectUris.Select(_ => _.ToString())],
                RedirectUri = [.. model.RedirectUris.Select(_ => _.ToString())]
            };
        }
    }
}
