/*using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;

namespace RoadReady.Repositories
{
    public class UserAuditRepository: IUserAuditRepository
    {
        private readonly RoadReadyContext _context;

        public UserAuditRepository(RoadReadyContext context)
        {
            _context = context;
        }

        public async Task<UserAudit> GetAuditByIdAsync(int id)
        {
            return await _context.UserAudits.FindAsync(id);
        }

        public async Task<IEnumerable<UserAudit>> GetAllAuditsAsync()
        {
            return await _context.UserAudits.ToListAsync();
        }

        public async Task AddAuditAsync(UserAudit audit)
        {
            await _context.UserAudits.AddAsync(audit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuditAsync(UserAudit audit)
        {
            _context.UserAudits.Update(audit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuditAsync(int id)
        {
            var audit = await GetAuditByIdAsync(id);
            if (audit != null)
            {
                _context.UserAudits.Remove(audit);
                await _context.SaveChangesAsync();
            }
        }
    }
}*/
