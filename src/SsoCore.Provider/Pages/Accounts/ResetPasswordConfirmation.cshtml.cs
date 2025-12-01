using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SsoCore.Provider.Pages.Accounts
{
    public class ResetPasswordConfirmation : PageModel
    {
        [BindProperty] public string? Email { get; set; }

        public void OnGet(string email)
        {
            Email = email;
        }
        
        public IActionResult OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                return Page();
            }
            
            return RedirectToPage("/Accounts/VerifyEmail",
                new { email = Email });
        }
    }
}

