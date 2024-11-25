using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IAdminActionsRepository
    {
        Task<AdminActions> GetAdminActionByIdAsync(int id);
        Task<IEnumerable<AdminActions>> GetAllAdminActionsAsync();
        Task AddAdminActionAsync(AdminActions action);
        Task UpdateAdminActionAsync(AdminActions action);
        Task DeleteAdminActionAsync(int id);
    }
}
