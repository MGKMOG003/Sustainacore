using Sustainacore.Application.Interfaces;
using Sustainacore.Domain.Entities;
using System.Threading.Tasks;

namespace Sustainacore.Application.Services;

public class ProjectService
{
    private readonly IProjectRepository _repo;
    public ProjectService(IProjectRepository repo) => _repo = repo;

    public async Task<Project> CreateProjectAsync(string name, string? description, string ownerId)
    {
        var project = new Project(name, description);
        project.UserIds.Add(ownerId);
        await _repo.AddAsync(project);
        return project;
    }

    public Task<IReadOnlyList<Project>> ListProjectsForUserAsync(string userId) => _repo.GetAllAsync(userId);
}
