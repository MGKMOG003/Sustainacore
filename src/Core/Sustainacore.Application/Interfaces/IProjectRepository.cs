using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Application.Interfaces
{
    /// <summary>
    /// Abstraction for project persistence.  For now the implementation can be in-memory,
    /// but later it can be backed by Firestore or SQL.
    /// </summary>
    public interface IProjectRepository
    {
        Task<IReadOnlyList<Project>> GetAllAsync(string userId);
        Task<Project?> GetByIdAsync(Guid id);
        Task AddAsync(Project project);
        Task AssignUserAsync(Guid projectId, string userId);
    }
}
