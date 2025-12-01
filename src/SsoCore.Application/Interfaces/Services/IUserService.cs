using SsoCore.Application.DTOs;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> CreateUser(UserDto user);
        Task<Pageable<UserDto>> FilterUsersAsync(string? query, int pageNumber, int pageSize);
        Task<Result<UserDto>> UpdateUser(string? id, string? firstName, string? lastName, string? middleNames, string updatedBy);
        Task<Result> DeActivateUser(string? id, string updatedBy);
        Task<Result<UserDto>> ActivateUser(string? id, string updatedBy);
        Task<Result<UserDto>> GetUserByIdAsync(string userId);
    }
}
