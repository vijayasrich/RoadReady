using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminActionsRepository : IAdminActionsRepository
{
    private readonly RoadReadyContext _context;

    public AdminActionsRepository(RoadReadyContext context)
    {
        _context = context;
    }

    public async Task<AdminActions> GetAdminActionByIdAsync(int id)
    {
        return await _context.AdminActions.FindAsync(id);
    }

    public async Task<IEnumerable<AdminActions>> GetAllAdminActionsAsync()
    {
        return await _context.AdminActions.ToListAsync();
    }

    public async Task AddAdminActionAsync(AdminActions adminAction)
    {
        await _context.AdminActions.AddAsync(adminAction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAdminActionAsync(AdminActions adminAction)
    {
        _context.AdminActions.Update(adminAction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAdminActionAsync(int id)
    {
        var adminAction = await GetAdminActionByIdAsync(id);
        if (adminAction != null)
        {
            _context.AdminActions.Remove(adminAction);
            await _context.SaveChangesAsync();
        }
    }
}

