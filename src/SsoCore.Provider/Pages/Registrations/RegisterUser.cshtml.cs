using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Infrastructure.Data.Identity;
using SsoCore.Provider.Constants;

namespace SsoCore.Provider.Pages.Registrations
{
    public class RegisterUserModel(
        UserManager<ApplicationUser> userManager,
        IClientService clientService,
        ILogger<RegisterUserModel> logger,
        IMapper mapper,
        IEmailService emailService) : PageModel
    {
        [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }

        [BindProperty(SupportsGet = true)] public string? ClientId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        [BindProperty] public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        [BindProperty] public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [BindProperty] public string? Email { get; set; }

        [BindProperty] public string CountryCode { get; set; } = "+234";

        [Phone(ErrorMessage = "Invalid phone number")]
        [BindProperty] public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [BindProperty] public string? Password { get; set; }

        [TempData] public string? Error { get; set; }
        [TempData] public string? Success { get; set; }

    public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ReturnUrl))
                {
                    Error = "Client ID and return URL are required.";
                    return RedirectToErrorPage(
                        title: "Invalid Request",
                        message: Error,
                        showRetry: false);
                }

                var (isValid, client) = await clientService.ValidateClientAndReturnUrl(ClientId, ReturnUrl);
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during registration page initialization");
                Error = "An error occurred while loading the registration page.";
                return RedirectToErrorPage(
                    title: "Initialization Error",
                    message: Error,
                    showRetry: true,
                    retryUrl: $"/Registrations/RegisterUser?clientId={ClientId}&returnUrl={ReturnUrl}");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ReturnUrl))
                {
                    Error = "Client ID and return URL are required.";
                    return Page();
                }

                if (string.IsNullOrEmpty(Password) || !IsPasswordValid(Password))
                {
                    ModelState.AddModelError("Password", 
                        "Password must be at least 8 characters long and contain at least one number and one uppercase letter.");
                    return Page();
                }

                if (string.IsNullOrEmpty(Email))
                {
                    Error = "Email is required.";
                    return Page();
                }

                var existingUser = await userManager.FindByEmailAsync(Email);
                if (existingUser != null)
                {
                    Error = "A user with this email already exists.";
                    return Page();
                }
                
                var user = ApplicationUser.Create(
                    email: Email!,
                    firstName: FirstName!,
                    lastName: LastName!,
                    createdBy: Email!, 
                    phoneNumber: !string.IsNullOrEmpty(PhoneNumber) ? $"{CountryCode}{PhoneNumber}" : null,
                    emailConfirmed: false
                );

                var result = await userManager.CreateAsync(user, Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                var userDto = mapper.Map<UserDto>(user);
                var otp = await userManager.GenerateTwoFactorTokenAsync(user, TokenProviderConstant.Email);
                var otpEmail = await userDto.RegistrationOtpEmailModel(otp);
                await emailService.SendAsync(otpEmail);

                return RedirectToPage("/Registrations/ConfirmEmail", new 
                { 
                    email = user.Email,
                    clientId = ClientId,
                    returnUrl = ReturnUrl
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during user registration");
                Error = "An error occurred during registration. Please try again.";
                return Page();
            }
        }

        private static bool IsPasswordValid(string password)
        {
            if (password.Length < 8) return false;
            if (!Regex.IsMatch(password, @"\d")) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            return true;
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