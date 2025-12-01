using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Data.Identity;
using SsoCore.Provider.Constants;

namespace SsoCore.Provider.Pages.Accounts
{
    public class ResetPasswordModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordModel> logger,
        IMapper mapper,
        IEmailService emailService
        ) : PageModel
    {
        [BindProperty] public string Email { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string Success { get; set; } = string.Empty;

        public ActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string generalError = "Invalid credentials";

            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    Error = "Email is required";
                    return Page();
                }

                var user = await userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    logger.LogWarning("Password reset attempt failed for email: {Email}", Email);
                    Error = generalError;
                    return Page();
                }

                var resetToken = await userManager.GenerateTwoFactorTokenAsync(user, TokenProviderConstant.Email);
                if (string.IsNullOrEmpty(resetToken))
                {
                    logger.LogError("Failed to generate reset token for user: {Email}", user.Email);
                    Error = "An error occurred while generating the verification code. Please try again.";
                    return Page();
                }

                var userDTO = mapper.Map<UserDto>(user);

                var emailModel = await userDTO.ResetPasswordEmailModel(resetToken);
                await emailService.SendAsync(emailModel);

                Success = "A 6-digit verification code has been sent to your email.";
                return RedirectToPage("/Accounts/ResetPasswordConfirmation",
                    new { email = user.Email });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while processing the password reset for email: {Email}", Email); Error = "An unexpected error occurred. Please try again later.";
                Error = "An error occurred while processing your request. Please try again.";
                return Page();
            }
        }
    }
}
