using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SsoCore.Application.Configurations;
using SsoCore.Application.Constants;
using SsoCore.Application.DTOs;
using SsoCore.Application.Helpers;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Data.Identity;
using SsoCore.Provider.Constants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace SsoCore.Provider.Pages.Accounts
{
    public class LoginModel(
        ILogger<LoginModel> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationSchemeProvider authenticationSchemeProvider,
        ConfigSettings configSettings,
        IMapper mapper,
        IEmailService emailService
        ) : PageModel
    {

        [BindProperty] public string? Provider { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
        [BindProperty] public string? ReturnUrl { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Success { get; set; } = string.Empty;
        public IEnumerable<AuthenticationScheme> ExternalProviders { get; set; } = [];

        public async Task<IActionResult> OnGet(string returnUrl)
        {
            ExternalProviders = await authenticationSchemeProvider.GetAllSchemesAsync();

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded && !string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            string generalError = "Invalid email or password.";

            ExternalProviders = await authenticationSchemeProvider.GetAllSchemesAsync();

            try
            {
                if (!string.IsNullOrEmpty(Provider))
                {
                    var properties = signInManager.ConfigureExternalAuthenticationProperties(Provider, ReturnUrl);
                    return Challenge(properties, authenticationSchemes: [Providers.Google]);
                }

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    Error = "Email and password are required";
                    return Page();
                }

                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    logger.LogWarning("User not found for email: {Email}", email);
                    Error = generalError;
                    return Page();
                }

                var userDTO = mapper.Map<UserDto>(user);
                if (!await userManager.IsEmailConfirmedAsync(user))
                {
                    string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    string confirmURL = new Uri(new Uri(configSettings.SSOSettings.Issuer!), $"{EmailTemplates.ConfirmAccountPath}?userId={user.Id}&code={code.UrlEncoded()}").ToString();
                    var emailModel = await userDTO.ConfirmAccountEmailModel(confirmURL);
                    await emailService.SendAsync(emailModel);

                    Error = "Email not confirmed. Please check your email for confirmation instructions.";
                    return Page();
                }

                var result = await signInManager.PasswordSignInAsync(user.Email!, password, RememberMe, lockoutOnFailure: true);


                if (result.IsLockedOut)
                {
                    logger.LogWarning("User account locked out for email: {Email}", Email);
                    Error = "Your account is locked due to multiple failed attempts. Please reset your password.";
                    return Page();
                }

                if (result.IsNotAllowed)
                {
                    logger.LogWarning("Password sign-in failed for email: {Email}", Email);
                    Error = generalError;
                    return Page();
                }


                if (!result.RequiresTwoFactor && result.Succeeded)
                {
                   // return RedirectToLocal(ReturnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, TokenProviderConstant.Email);

                    if (string.IsNullOrEmpty(token))
                    {
                        logger.LogError("Failed to generate OTP token for user: {Email}", email);
                        Error = generalError;
                        return Page();
                    }

                    var emailModel = await userDTO.LoginOtpEmailModel(token);
                    await emailService.SendAsync(emailModel);

                    return RedirectToPage("/Accounts/VerifyOTP", new
                    {
                        returnUrl = ReturnUrl,
                        user.Email,
                        rememberMe = RememberMe
                    });
                }

                Error = generalError;
                logger.LogWarning("Unknown login failure for email: {Email}", email);
                return Page();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(LoginModel)} => An error occurred while authenticating");
                Error = generalError;
                return Page();
            }
        }
    }
}