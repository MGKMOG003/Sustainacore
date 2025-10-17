using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainacore.Application.Services;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Web.Areas.Admin.Pages.Users
{
    [Authorize(Policy = "AdminOnly")]
    public class EditUserModel : PageModel
    {
        private readonly UserService _service;
        public User? UserItem { get; private set; }

        public EditUserModel(UserService service)
        {
            _service = service;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel { public string Role { get; set; } = string.Empty; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            UserItem = await _service.GetByIdAsync(id);
            if (UserItem == null) return NotFound();
            Input.Role = UserItem.Role;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            UserItem = await _service.GetByIdAsync(id);
            if (UserItem == null) return NotFound();
            await _service.UpdateRoleAsync(id, Input.Role);
            return RedirectToPage("/Users/Index");
        }
    }
}
