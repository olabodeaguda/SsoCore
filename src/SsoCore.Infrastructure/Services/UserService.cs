using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SsoCore.Application.Configurations;
using SsoCore.Application.Constants;
using SsoCore.Application.DTOs;
using SsoCore.Application.Helpers;
using SsoCore.Application.Interfaces.Repositories;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;
using SsoCore.Domain.Errors;
using SsoCore.Infrastructure.Data.Identity;

namespace SsoCore.Infrastructure.Services
{
    public class UserService(IRepository<ApplicationUser> userRepository,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        ILogger<UserService> logger,
        IEmailService emailService,
        ConfigSettings configSettings) : IUserService
    {
        public async Task<Result<UserDto>> CreateUser(UserDto model)
        {
            try
            {
                var applicationUser = mapper.Map<ApplicationUser>(model);
                var user = await userManager.FindByEmailAsync(model.Email!);
                if (user != null)
                {
                    logger.LogError("User with email {Email} already exists", model.Email);
                    return Result<UserDto>.Fail(UserError.AlreadyExist);
                }

                applicationUser.Enable2Fa();
                applicationUser.SetCreatedBy(model.CreatedBy ?? "");
                var result = await userManager.CreateAsync(applicationUser);
                if (!result.Succeeded)
                {
                    logger.LogError("Error creating user. Errors: {Errors}", result.Errors);
                    return Result<UserDto>.Fail(UserError.CreateFailed);
                }

                var userDTO = mapper.Map<UserDto>(applicationUser);

                string code = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                string confirmURL = new Uri(new Uri(configSettings.SSOSettings.Issuer!), $"{EmailTemplates.ConfirmAccountPath}?userId={applicationUser.Id}&code={code.UrlEncoded()}").ToString();
                var emailModel = await userDTO.ConfirmAccountEmailModel(confirmURL);
                await emailService.SendAsync(emailModel);

                return Result<UserDto>.Success(userDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating user");
                return Result<UserDto>.Fail(UserError.CreateFailed);
            }
        }

        public async Task<Pageable<UserDto>> FilterUsersAsync(string? query, int pageNumber, int pageSize)
        {
            query ??= string.Empty;
            var q = query.Split(" ").ToList();

            IQueryable<ApplicationUser> users = userRepository.Query();

            if (q.Count > 0)
            {
                foreach (var i in q)
                {
                    users = users.Where(u =>
                                (u.FirstName != null && u.FirstName.Contains(i)) ||
                                (u.LastName != null && u.LastName.Contains(i)) ||
                                (u.Email != null && u.Email.Contains(i)) ||
                                (u.MiddleNames != null && u.MiddleNames.Contains(i))
                            ); 
                }
            }

            var result = await userRepository.GetAllAsync(
                filter: q => users,
                pageNumber: pageNumber,
                pageSize: pageSize
            );


            return Pageable<UserDto>.Create(mapper.Map<List<UserDto>>(result.Data),
                result.TotalItems,
                result.CurrentPage,
                result.PageSize);
        }

        public async Task<Result<UserDto>> UpdateUser(string? id, string? firstName, string? lastName, string? middleNames, string updatedBy)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id!);
                if (user == null)
                {
                    logger.LogError("User with ID {UserId} not found", id);
                    return Result<UserDto>.Fail(UserError.NotFound);
                }

                user.Update(firstName, lastName, middleNames, updatedBy);

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    logger.LogError("Error updating user with ID {UserId}. Errors: {Errors}", id, result.Errors);
                    return Result<UserDto>.Fail(UserError.UpdateFailed);
                }

                return Result<UserDto>.Success(mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating user");
                return Result<UserDto>.Fail(UserError.UpdateFailed);
            }
        }

        public async Task<Result> DeActivateUser(string? id, string updatedBy)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id!);
                if (user == null)
                {
                    logger.LogError("User with ID {UserId} not found", id);
                    return Result.Fail(UserError.NotFound);
                }
                user.IsDisabled = true;
                user.SetLastUpdatedBy(updatedBy);
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    logger.LogError("Error deactivating user with ID {UserId}. Errors: {Errors}", id, result.Errors);
                    return Result.Fail(UserError.DeActivateFailed);
                }

                return Result.Success("User deactivated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deactivating user");
                return Result.Fail(UserError.DeActivateFailed);
            }
        }

        public async Task<Result<UserDto>> ActivateUser(string? id, string updatedBy)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id!);
                if (user == null)
                {
                    logger.LogError("User with ID {UserId} not found", id);
                    return Result<UserDto>.Fail(UserError.NotFound);
                }

                user.IsDisabled = false;
                user.SetLastUpdatedBy(updatedBy);

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    logger.LogError("Error activating user with ID {UserId}. Errors: {Errors}", id, result.Errors);
                    return Result<UserDto>.Fail(UserError.ActivateFailed);
                }

                return Result<UserDto>.Success(mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error activating user");
                return Result<UserDto>.Fail(UserError.ActivateFailed);
            }
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogError("User {userId} not found", userId);
                return Result<UserDto>.Fail(UserError.NotFound);
            }
            return Result<UserDto>.Success(mapper.Map<UserDto>(user));
        }
    }
}
