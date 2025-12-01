using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SsoCore.Provider.Pages.Accounts
{
    public class ChangePasswordConfirmation : PageModel
    {
        [BindProperty] public string? Email { get; set; }

        public void OnGet(string email)
        {
            Email = email;
        }
    }
}
