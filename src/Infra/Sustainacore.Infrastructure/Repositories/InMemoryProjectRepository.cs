using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sustainacore.Application.Interfaces;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory repository seeded with demo users.  Replace with actual Firebase-admin implementation later.
    /// </summary>
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new()
        {
            new() { Id = "1", Email = "admin1@sustainacore.demo",      DisplayName = "Admin One",      Role = "Admin" },
            new() { Id = "2", Email = "admin2@sustainacore.demo",      DisplayName = "Admin Two",      Role = "Admin" },
            new() { Id = "3", Email = "pm1@sustainacore.demo",         DisplayName = "PM One",         Role = "ProjectManager" },
            new() { Id = "4", Email = "pm2@sustainacore.demo",         DisplayName = "PM Two",         Role = "ProjectManager" },
            new() { Id = "5", Email = "contractor1@sustainacore.demo", DisplayName = "Contractor One", Role = "Contractor" },
            new() { Id = "6", Email = "contractor2@sustainacore.demo", DisplayName = "Contractor Two", Role = "Contractor" },
            new() { Id = "7", Email = "client1@sustainacore.demo",     DisplayName = "Client One",     Role = "Client" },
            new() { Id = "8", Email = "client2@sustainacore.demo",     DisplayName = "Client Two",     Role = "Client" }
        };

        public Task<IReadOnlyList<User>> GetAllAsync() => Task.FromResult((IReadOnlyList<User>)_users);

        public Task<User?> GetByIdAsync(string userId) => Task.FromResult(_users.SingleOrDefault(u => u.Id == userId));

        public Task UpdateRoleAsync(string userId, string newRole)
        {
            var user = _users.SingleOrDefault(u => u.Id == userId);
            if (user != null) user.Role = newRole;
            return Task.CompletedTask;
        }
    }
}
