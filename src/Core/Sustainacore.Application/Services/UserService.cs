using System.Collections.Generic;
using System.Threading.Tasks;
using Sustainacore.Application.Interfaces;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Application.Services
{
    /// <summary>
    /// Application-level logic for user management.  Use this in controllers/pages.
    /// </summary>
    public class UserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyList<User>> ListUsersAsync() => _repo.GetAllAsync();
        public Task<User?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);
        public Task UpdateRoleAsync(string userId, string newRole) => _repo.UpdateRoleAsync(userId, newRole);
    }
}
