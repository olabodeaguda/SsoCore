using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SsoCore.Provider.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel(ILogger<ErrorModel> logger) : PageModel
    {
        public string? RequestId { get; set;  }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        public string ErrorTitle { get; set; } = "Something went wrong";
        public string ErrorMessage { get; set; } = "An unexpected error occurred while processing your request.";
        public bool ShowRetry { get; set; }
        public string? RetryUrl { get; set; } = string.Empty;

        public void OnGet(string? title = null, string? message = null, bool? showRetry = null, string? retryUrl = null)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            if (!string.IsNullOrEmpty(title))
                ErrorTitle = title;
            
            if (!string.IsNullOrEmpty(message))
                ErrorMessage = message;
            
            ShowRetry = showRetry ?? false;
            RetryUrl = retryUrl ?? string.Empty;
            
            logger.LogError("Error page displayed - RequestId: {RequestId}, Title: {ErrorTitle}, Message: {ErrorMessage}", 
                RequestId, ErrorTitle, ErrorMessage);
        }
    }

}