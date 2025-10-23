using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sustainacore.Web.Controllers
{
    public class AccountController : Controller
    {
        // -------- DEMO USER STORE (replace with real DB/Identity later) ----------
        private static readonly Dictionary<string, (string Password, string Role)> Users = new()
        {
            // email                              password     role
            ["admin@sustainacore.local"]          = ("Admin123!",          "Admin"),
            ["admin2@sustainacore.local"]         = ("Admin123!",          "Admin"),
            ["pm@sustainacore.local"]             = ("Pm123!",             "ProjectManager"),
            ["pm2@sustainacore.local"]            = ("Pm123!",             "ProjectManager"),
            ["contractor@sustainacore.local"]     = ("Contractor123!",     "Contractor"),
            ["contractor2@sustainacore.local"]    = ("Contractor123!",     "Contractor"),
            ["client@sustainacore.local"]         = ("Client123!",         "Client"),
            ["client2@sustainacore.local"]        = ("Client123!",         "Client"),
        };
        // ------------------------------------------------------------------------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // Prevent loops by only accepting local ReturnUrls
            ViewData["ReturnUrl"] = (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : null;
            return View(); // Views/Account/Login.cshtml
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            if (Users.TryGetValue(model.Email.Trim().ToLowerInvariant(), out var user) &&
                model.Password == user.Password) // demo only
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, model.Email),
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // If a valid local returnUrl exists, go there; otherwise route by role
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return Redirect(GetDashboardUrlFor(user.Role));
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(); // Views/Account/Register.cshtml (can be a simple form now)
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var key = model.Email.Trim().ToLowerInvariant();
            if (Users.ContainsKey(key))
            {
                ModelState.AddModelError(string.Empty, "Email already registered.");
                return View(model);
            }

            // Auto-register as Client (per your requirement)
            Users[key] = (model.Password, "Client");
            TempData["JustRegistered"] = "Account created. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View(); // optional page

        private RedirectResult Redirect(string url) => new(url);

        private string GetDashboardUrlFor(string role) => role switch
        {
            "Admin"          => "/Admin",              // e.g., Areas/Admin/Pages/Index or /Admin/Home/Index
            "ProjectManager" => "/ProjectManager",     // e.g., Areas/ProjectManager/Pages/Index
            "Contractor"     => "/Contractor",
            "Client"         => "/Client",
            _                => Url.Action("Index", "Home") ?? "/Home/Index"
        };

        // --------------------- View Models ---------------------
        public class LoginVm
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public bool RememberMe { get; set; }
        }

        public class RegisterVm
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public string ConfirmPassword { get; set; } = "";
        }
    }
}
