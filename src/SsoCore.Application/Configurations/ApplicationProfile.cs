using AutoMapper;
using SsoCore.Application.DTOs;
using SsoCore.Application.Handlers.Commands;

namespace SsoCore.Application.Configurations
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<CreateUserRequest, UserDto>();
            CreateMap<UpdateScopeRequest, ScopeDto>();
            CreateMap<CreateScopeRequest, ScopeDto>();
            CreateMap<CreateClientRequest, ClientDto>();
            CreateMap<PatchClientRequest, ClientDto>();
            CreateMap<UpdateClientRequest, ClientDto>();
        }
    }
}
