using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sustainacore.Application.Interfaces;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Infrastructure.Repositories
{
    /// <summary>
    /// Development-only in-memory repository for projects.  Replace this with
    /// a database implementation for production.
    /// </summary>
    public class InMemoryProjectRepository : IProjectRepository
    {
        private readonly List<Project> _projects = new();

        public Task AddAsync(Project project)
        {
            _projects.Add(project);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Project>> GetAllAsync(string userId)
        {
            var projects = _projects.Where(p => p.UserIds.Contains(userId)).ToList();
            return Task.FromResult((IReadOnlyList<Project>)projects);
        }

        public Task<Project?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_projects.SingleOrDefault(p => p.Id == id));
        }

        public Task AssignUserAsync(Guid projectId, string userId)
        {
            var project = _projects.SingleOrDefault(p => p.Id == projectId);
            if (project != null && !project.UserIds.Contains(userId))
                project.UserIds.Add(userId);

            return Task.CompletedTask;
        }
    }
}
