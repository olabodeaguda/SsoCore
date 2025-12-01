using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Data.Identity;
using SsoCore.Provider.Constants;

namespace SsoCore.Provider.Pages.Registrations
{
    public class ConfirmEmailModel(UserManager<ApplicationUser> userManager, 
        ILogger<ConfirmEmailModel> logger,
        IClientService clientService,
        IMapper mapper,
        IEmailService emailService
        ) : PageModel
    {
        [BindProperty(SupportsGet = true)] public string Email { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string? ClientId { get; set; }
        [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }
        
        [Required(ErrorMessage = "OTP is required")]
        [BindProperty] public string[] OTP { get; set; } = new string[6];
        public string Error { get; set; } = string.Empty;
        public string Success { get; set; } = string.Empty;
        
        
        public async Task<IActionResult> OnGetAsync(string email, string clientId, string returnUrl)
        {
            Email = email;
            ClientId = clientId;
            ReturnUrl = returnUrl;
            
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ReturnUrl))
            {
                Error = "Invalid confirmation link.";
                return Page();
            }
            
            var (isValid, client) = await clientService.ValidateClientAndReturnUrl(clientId, returnUrl);
            if (!isValid)
            {
                Error = "Invalid client or return URL.";
                return RedirectToErrorPage(
                    title: "Invalid Request",
                    message: Error,
                    showRetry: false);
            }
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (OTP == null || OTP.Length == 0)
                {
                    Error = "OTP is required.";
                    return Page();
                }

                var otp = string.Join("", OTP);
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

                return RedirectToPage("/Registrations/RegistrationConfirmation", new { clientId = ClientId, returnUrl = ReturnUrl });
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

                var userModel = mapper.Map<UserDto>(user);

                var emailModel = await userModel.LoginOtpEmailModel(newOtp);
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
        
        private IActionResult RedirectToErrorPage(string title, string message, bool showRetry, string? retryUrl = null)
        {
            return RedirectToPage("/Error", new 
            {
                title = title,
                message = message,
                showRetry = showRetry,
                retryUrl = retryUrl
            });
        }
    }
}
