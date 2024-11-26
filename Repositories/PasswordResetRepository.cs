using RoadReady.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using RoadReady.Authentication;

namespace RoadReady.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly RoadReadyContext _context;

        public PasswordResetRepository(RoadReadyContext context)
        {
            _context = context;
        }

        // Get PasswordReset by ID
        public async Task<PasswordReset> GetPasswordResetByIdAsync(int id)
        {
            return await _context.PasswordResets
                                 .FirstOrDefaultAsync(pr => pr.ResetId == id);
        }

        // Add a new PasswordReset record
        public async Task AddPasswordResetAsync(PasswordReset passwordReset)
        {
            await _context.PasswordResets.AddAsync(passwordReset);
            await _context.SaveChangesAsync();
        }

        // Update an existing PasswordReset record
        public async Task UpdatePasswordResetAsync(PasswordReset passwordReset)
        {
            _context.PasswordResets.Update(passwordReset);
            await _context.SaveChangesAsync();
        }

        // Delete a PasswordReset record by ID
        public async Task DeletePasswordResetAsync(int id)
        {
            var passwordReset = await _context.PasswordResets
                                               .FirstOrDefaultAsync(pr => pr.ResetId == id);
            if (passwordReset != null)
            {
                _context.PasswordResets.Remove(passwordReset);
                await _context.SaveChangesAsync();
            }
        }
    }
}
