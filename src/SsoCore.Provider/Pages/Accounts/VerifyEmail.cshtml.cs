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
    public class VerifyEmailModel(
        UserManager<ApplicationUser> userManager, 
        ILogger<VerifyEmailModel> logger,
        IMapper mapper,
        IEmailService emailService
        ) : PageModel
    {
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string[] OTP { get; set; } = new string[6];
        public string Error { get; set; } = string.Empty;
        public string Success { get; set; } = string.Empty;

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var otp = string.Join("", OTP);

            try
            {
                if (OTP == null || OTP.Length == 0)
                {
                    Error = "OTP is required.";
                    return Page();
                }

                var user = await userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    Error = "Invalid credentials";
                    return Page();
                }

                var isValidOtp = await userManager.VerifyTwoFactorTokenAsync(user, TokenProviderConstant.Email, otp);
                if (!isValidOtp)
                {
                    Error = "Invalid or expired OTP.";
                    return Page();
                }

                return RedirectToPage("/Accounts/ChangePassword", new { email = Email });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while verifying the OTP for email: {Email}", Email);
                Error = "An error occurred while verifying your OTP. Please try again.";
                return Page();
            }

        }
        
        
        public async Task<IActionResult> OnPostResendOtpAsync()
        {
            try
            {
                var user = await userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    Error = "Invalid credentials";
                    return Page();
                }

                var newOtp = await userManager.GenerateTwoFactorTokenAsync(user, TokenProviderConstant.Email);

                var userDTO = mapper.Map<UserDto>(user);

                var emailModel = await userDTO.LoginOtpEmailModel(newOtp);
                await emailService.SendAsync(emailModel);

                Success = "A new OTP has been sent to your email.";
                return Page();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while verifying the OTP for email: {Email}", Email);
                Error = "An error occurred while verifying your OTP. Please try again.";
                return Page();
            }
        }
    }
}
