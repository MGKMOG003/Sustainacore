using System.Collections.Generic;
using System.Threading.Tasks;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Application.Interfaces
{
    /// <summary>
    /// Defines basic persistence operations for user management.
    /// A later implementation could wrap Firebase Admin or a database.
    /// </summary>
    public interface IUserRepository
    {
        Task<IReadOnlyList<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string userId);
        Task UpdateRoleAsync(string userId, string newRole);
    }
}

