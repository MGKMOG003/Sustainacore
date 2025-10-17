using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sustainacore.Web.Areas.Admin.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminModel : PageModel
    {
        public void OnGet() { }
    }
}
