using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterModel : PageModel
{
    private readonly FirebaseAuthService _auth;

    [BindProperty] public string DisplayName { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";

    public RegisterModel(FirebaseAuthService auth) => _auth = auth;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var (ok, msg, token, display) = await _auth.RegisterAsync(Email, Password, DisplayName);
        if (!ok || string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError(string.Empty, "Registration failed.");
            return Page();
        }

        // New users default to Client; custom claim set by Admin later
        Response.Cookies.Append("id_token", token, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });
        return RedirectToPage("/Index", new { area = "Client" });
    }
}

