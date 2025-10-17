using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sustainacore.Api.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    public string? UserName { get; private set; }
    public void OnGet() => UserName = User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
}
