using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Data.Identity;
using SsoCore.Provider.Constants;

namespace SsoCore.Provider.Pages.Accounts
{
    public class VerifyOTPModel(
        ILogger<VerifyOTPModel> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> _signInManager,
        IMapper mapper,
        IEmailService emailService
        ) : PageModel
    {
        [BindProperty] public string[] OTP { get; set; } = new string[6];
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string? ReturnUrl { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Success { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;

        public async Task OnGet(string email, string returnUrl, bool rememberMe)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                logger.LogError("Unable to load two-factor authentication user.");
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
            }
            Console.WriteLine($"ReturnUrl: {returnUrl}");

            Email = email;
            ReturnUrl = returnUrl;
            RememberMe = rememberMe;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Error = "Invalid form submission.";
                return Page();
            }

            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    logger.LogError("Unable to load user after 2FA.");
                    return RedirectToPage("/Account/Login");
                }

                var result = await _signInManager.TwoFactorSignInAsync(TokenProviderConstant.Email, string.Join("", OTP), RememberMe, RememberMe);

                if (!result.Succeeded)
                {
                    Error = "Invalid authenticator code.";
                    return Page();
                }

                if (result.IsLockedOut)
                {
                    return RedirectToPage("/Account/Lockout");
                }

                var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, Email)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                var decodedReturnUrl = Uri.UnescapeDataString(ReturnUrl ?? "");
                if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
                {
                    return Redirect(decodedReturnUrl);
                }
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during OTP verification for email: {Email}", Email);
                Error = "An unexpected error occurred. Please try again.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResendOtpAsync()
        {
            ViewData["ReturnUrl"] = ReturnUrl;
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
                return RedirectToPage(new { returnUrl = ReturnUrl });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while verifying the OTP for email: {Email}", Email);
                Error = "An error occurred while verifying your OTP. Please try again.";
                return RedirectToPage(new { returnUrl = ReturnUrl });
            }
        }
    }
}
