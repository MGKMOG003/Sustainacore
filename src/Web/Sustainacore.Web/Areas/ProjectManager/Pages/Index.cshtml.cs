using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainacore.Application.Services;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Web.Areas.ProjectManager.Pages
{
    [Authorize(Policy = "PMOnly")]
    public class IndexModel : PageModel
    {
        private readonly ProjectService _service;
        public IReadOnlyList<Project> Projects { get; private set; } = new List<Project>();

        public IndexModel(ProjectService service)
        {
            _service = service;
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst("user_id")?.Value!;
            Projects = await _service.ListProjectsForUserAsync(userId);
        }
    }
}
