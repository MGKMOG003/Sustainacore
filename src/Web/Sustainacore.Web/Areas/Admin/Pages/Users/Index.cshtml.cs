using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainacore.Application.Services;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Web.Areas.Admin.Pages.Users
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly UserService _service;
        public IReadOnlyList<User> Users { get; private set; } = new List<User>();

        public IndexModel(UserService service)
        {
            _service = service;
        }

        public async Task OnGetAsync()
        {
            Users = await _service.ListUsersAsync();
        }
    }
}
