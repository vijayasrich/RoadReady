using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IPasswordResetRepository
    {
        Task<PasswordReset> GetPasswordResetByTokenAsync(string token);
        Task AddPasswordResetAsync(PasswordReset passwordReset);
        Task UpdatePasswordResetAsync(PasswordReset passwordReset);
        Task DeletePasswordResetAsync(string token);
    }
}
