using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sustainacore.Web.Controllers
{
    [Authorize] // require login for the app’s main content
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // For now, a simple landing page after login
            return View(); // Views/Home/Index.cshtml
        }
    }
}
