using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Data.Identity;

namespace SsoCore.Provider.Pages.Accounts
{
    public class ChangePasswordModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ChangePasswordModel> logger,
        IConfiguration configuration,
        IMapper mapper,
        IEmailService emailService
        ) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;

        [BindProperty] public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string Success { get; set; } = string.Empty;

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    Error = "All fields are required.";
                    return Page();
                }

                if (NewPassword != ConfirmPassword)
                {
                    Error = "Passwords do not match.";
                    return Page();
                }

                var user = await userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    Error = "Invalid credentials";
                    return Page();
                }

                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(resetToken))
                {
                    logger.LogError("Failed to generate token for user: {Email}", user.Email);
                    Error = "An error occurred while generating the verification code. Please try again.";
                    return Page();
                }

                var result = await userManager.ResetPasswordAsync(user, resetToken, NewPassword);
                if (!result.Succeeded)
                {
                    Error = "Password reset failed. Please try again.";
                    return Page();
                }

                var userDTO = mapper.Map<UserDto>(user);

                var loginUrl = _configuration["BaseUrl"];
                var emailModel = await userDTO.ResetPasswordSuccessfulEmailModel(loginUrl ?? "");

                await emailService.SendAsync(emailModel);

                Success = "Your password has been reset successfully.";
                return Redirect("/Accounts/ChangePasswordConfirmation");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while resetting your password for email: {Email}", Email);
                Error = "An error occurred while resetting your password. Please try again.";
                return Page();
            }

        }
    }
}