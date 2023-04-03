using ITSAPI.Models;

namespace ITSAPI.Interfaces
{
    public interface IProjectsService
    {
        Task<IEnumerable<Project>> GetProjectsAsync(string query);

        Task AddProjectAsync(Project project);

        Task DeleteProjectAsync(string id);

        Task UpdateProjectAsync(Project project);
    }
}
