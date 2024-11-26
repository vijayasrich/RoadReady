using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IPasswordResetRepository
    {
       
        Task<PasswordReset> GetPasswordResetByIdAsync(int id);
        Task AddPasswordResetAsync(PasswordReset passwordReset);
        Task UpdatePasswordResetAsync(PasswordReset passwordReset);
        Task DeletePasswordResetAsync(int id);
    }
}
