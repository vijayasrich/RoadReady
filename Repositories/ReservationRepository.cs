using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReservationRepository : IReservationRepository
{
    private readonly RoadReadyContext _context;

    public ReservationRepository(RoadReadyContext context)
    {
        _context = context;
    }

    public async Task<Reservation> GetReservationByIdAsync(int id)
    {
        return await _context.Reservations.FindAsync(id);
    }

    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        return await _context.Reservations.ToListAsync();
    }

    public async Task AddReservationAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReservationAsync(Reservation reservation)
    {
        // Check if a conflicting entity is already tracked
        var trackedEntity = _context.ChangeTracker.Entries<Reservation>()
                                    .FirstOrDefault(e => e.Entity.ReservationId == reservation.ReservationId);

        if (trackedEntity != null)
        {
            // Detach the tracked entity to avoid conflicts
            trackedEntity.State = EntityState.Detached;
        }

        // Update the reservation
        _context.Reservations.Update(reservation);

        // Save changes to the database
        await _context.SaveChangesAsync();
    }


    public async Task DeleteReservationAsync(int id)
    {
        var reservation = await GetReservationByIdAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
