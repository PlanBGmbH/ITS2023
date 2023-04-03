using ITSAPI.Models;

namespace ITSAPI.Interfaces
{
    public interface IResourcesService
    {
        Task<IEnumerable<Resource>> GetResourcesAsync(string query);

        Task AddResourceAsync(Resource resource);

        Task DeleteResourceAsync(string id);

        Task UpdateResourceAsync(Resource resource);
    }
}
