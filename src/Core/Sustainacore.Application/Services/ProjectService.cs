using System.Collections.Generic;
using System.Threading.Tasks;
using Sustainacore.Application.Interfaces;
using Sustainacore.Domain.Entities;

namespace Sustainacore.Application.Services
{
    /// <summary>
    /// Handles business logic around projects.  This sits in the Application layer.
    /// </summary>
    public class ProjectService
    {
        private readonly IProjectRepository _repo;

        public ProjectService(IProjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<Project> CreateProjectAsync(string name, string? description, string ownerId)
        {
            var project = new Project(name, description);
            project.UserIds.Add(ownerId);
            await _repo.AddAsync(project);
            return project;
        }

        public Task<IReadOnlyList<Project>> ListProjectsForUserAsync(string userId) => _repo.GetAllAsync(userId);
    }
}
