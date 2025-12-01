using SsoCore.Application.DTOs;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Interfaces.Services
{
    public interface IClientService
    {
        ClientMetadataDto GetMetadata();
        Result ValidateMetadata(ClientDto model);
        Task<Result<ClientDto>> CreateAsync(ClientDto client);
        Task<Result<ClientDto>> GetByClientId(string clientId);
        Task<ClientDto[]> GetByClientId(List<string> resources);
        Task<Result<ClientDto>> UpdateAsync(ClientDto clientDTO);
        Task<Result<ClientDto>> UpdateSecretAsync(string clientId, string secret);
        Task<Pageable<ClientDto>> FilterAsync(string? search, int pageSize, int pageNumber);
        Task<(bool isValid, ClientDto? client)> ValidateClientAndReturnUrl(string clientId, string returnUrl);
    }
}
