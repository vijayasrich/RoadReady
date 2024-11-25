using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Exceptions;
using RoadReady.Models;

namespace RoadReady.Repositories
{
    public class PasswordResetRepository: IPasswordResetRepository
    {
        private readonly RoadReadyContext _context;

        public PasswordResetRepository(RoadReadyContext context)
        {
            _context = context;
        }

        public async Task<PasswordReset> GetPasswordResetByTokenAsync(string token)
        {
            return await _context.PasswordResets
                                 .FirstOrDefaultAsync(pr => pr.Token == token);
        }

        public async Task AddPasswordResetAsync(PasswordReset passwordReset)
        {
            await _context.PasswordResets.AddAsync(passwordReset);
            await _context.SaveChangesAsync();
        }


        public async Task UpdatePasswordResetAsync(PasswordReset passwordReset)
        {
            var existingReset = await _context.PasswordResets.FindAsync(passwordReset.ResetId);
            if (existingReset == null)
            {
                throw new NotFoundException($"Password reset with ID {passwordReset.ResetId} was not found.");
            }

            // Update properties selectively if needed
            existingReset.Token = passwordReset.Token;
            existingReset.RequestTime = passwordReset.RequestTime;
            existingReset.ResetTime = passwordReset.ResetTime;

            _context.PasswordResets.Update(existingReset);
            await _context.SaveChangesAsync();
        }


        public async Task DeletePasswordResetAsync(string token)
        {
            var reset = await GetPasswordResetByTokenAsync(token);
            if (reset != null)
            {
                _context.PasswordResets.Remove(reset);
                await _context.SaveChangesAsync();
            }
        }
    }
}
