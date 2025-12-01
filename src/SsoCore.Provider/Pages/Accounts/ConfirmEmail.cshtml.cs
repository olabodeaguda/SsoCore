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
    public class ConfirmEmailModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ConfirmEmailModel> logger,
        IConfiguration configuration,
        IMapper mapper,
        IEmailService emailService
        ) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;

        [BindProperty(SupportsGet = true)] public string UserId { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)] public string Code { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Error { get; set; } = string.Empty;
        public string Success { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Code))
            {
                Error = "Invalid confirmation link.";
                return Page();
            }

            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                Error = "Invalid credentials";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                Error = "Invalid credentials";
                return Page();
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token))
            {
                logger.LogError("Failed to generate token for user: {Email}", user.Email);
                Error = "An error occurred while generating the verification code. Please try again.";
                return Page();
            }

            var result = await userManager.ResetPasswordAsync(user, token, NewPassword);

            if (result.Succeeded)
            {
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);

                    var userDTO = mapper.Map<UserDto>(user);

                    var loginUrl = _configuration["BaseUrl"];
                    var emailModel = await userDTO.ConfirmAccountSuccessfulEmailModel(loginUrl?? "");

                    await emailService.SendAsync(emailModel);
                }

                Success = "Your password has been set successfully. You can now log in.";
                return Redirect("/");
            }

            Error = "An error occurred while setting your password.";
            return Page();
        }
    }
}
