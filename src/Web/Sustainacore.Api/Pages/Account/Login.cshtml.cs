using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    private readonly FirebaseAuthService _auth;

    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";

    public LoginModel(FirebaseAuthService auth) => _auth = auth;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var (ok, msg, token, display) = await _auth.LoginAsync(Email, Password);
        if (!ok || string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return Page();
        }

        Response.Cookies.Append("id_token", token, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax });

        var role = DecodeRole(token) ?? "Client";
        return RedirectToDashboard(role);
    }

    private static string? DecodeRole(string jwt)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        return token.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
    }

    private IActionResult RedirectToDashboard(string role) => role switch
    {
        "Admin" => RedirectToPage("/Index", new { area = "Admin" }),
        "ProjectManager" => RedirectToPage("/Index", new { area = "ProjectManager" }),
        "Contractor" => RedirectToPage("/Index", new { area = "Contractor" }),
        _ => RedirectToPage("/Index", new { area = "Client" })
    };
}
