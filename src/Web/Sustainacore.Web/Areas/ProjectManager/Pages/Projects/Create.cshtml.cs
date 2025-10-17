using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainacore.Application.Services;

namespace Sustainacore.Web.Areas.ProjectManager.Pages.Projects
{
    [Authorize(Policy = "PMOnly")]
    public class CreateProjectModel : PageModel
    {
        private readonly ProjectService _service;

        public CreateProjectModel(ProjectService service)
        {
            _service = service;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var userId = User.FindFirst("user_id")?.Value!;
            await _service.CreateProjectAsync(Input.Name, Input.Description, userId);
            return RedirectToPage("/Index");
        }
    }
}
