using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SsoCore.Application.Interfaces.Services;

namespace SsoCore.Provider.Pages.Registrations
{
    public class RegistrationConfirmation(IClientService clientService) : PageModel
    {
        [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }
        [BindProperty(SupportsGet = true)] public string? ClientId { get; set; }
        public string Error { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string clientId, string returnUrl)
        {
            ReturnUrl = returnUrl;
            ClientId = clientId;
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(returnUrl))
            {
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

        public IActionResult OnPostAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ReturnUrl))
                {
                    Error = "Client ID and return URL are required.";
                    return Page();
                }

                return Redirect(ReturnUrl);
            }
            catch (Exception ex)
            {
                Error = "An error occurred while processing your request.";
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
