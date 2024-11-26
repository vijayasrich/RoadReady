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
        return await _context.Reservations
                         .Include(r => r.Extras)  // Include the CarExtras in the query
                         .FirstOrDefaultAsync(r => r.ReservationId == id);
    }

    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        return await _context.Reservations
           .Include(r => r.Extras) // Include CarExtras for each reservation
           .ToListAsync();
    }

    public async Task AddReservationAsync(Reservation reservation)
    {
        // Load CarExtras based on the CarExtraIds
        if (reservation.CarExtraIds != null && reservation.CarExtraIds.Any())
        {
            var selectedExtras = await _context.CarExtras
                                               .Where(extra => reservation.CarExtraIds.Contains(extra.ExtraId))
                                               .ToListAsync();

            // Add the selected CarExtras to the reservation
            foreach (var extra in selectedExtras)
            {
                reservation.Extras.Add(extra);
            }
        }

        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateReservationAsync(Reservation reservation)
    {
        // Retrieve the existing reservation
        var existingReservation = await _context.Reservations
                                                 .Include(r => r.Extras) // Load existing extras
                                                 .FirstOrDefaultAsync(r => r.ReservationId == reservation.ReservationId);

        if (existingReservation == null)
        {
            throw new Exception("Reservation not found.");
        }

        // Remove existing CarExtras not in the new CarExtraIds list
        var extrasToRemove = existingReservation.Extras
                                               .Where(extra => !reservation.CarExtraIds.Contains(extra.ExtraId))
                                               .ToList();

        foreach (var extra in extrasToRemove)
        {
            existingReservation.Extras.Remove(extra);
        }

        // Add new CarExtras that are not in the existing list
        var extrasToAdd = await _context.CarExtras
                                         .Where(extra => reservation.CarExtraIds.Contains(extra.ExtraId) &&
                                                        !existingReservation.Extras.Any(e => e.ExtraId == extra.ExtraId))
                                         .ToListAsync();

        foreach (var extra in extrasToAdd)
        {
            existingReservation.Extras.Add(extra);
        }

        // Update other fields if necessary
        existingReservation.PickupDate = reservation.PickupDate;
        existingReservation.DropoffDate = reservation.DropoffDate;
        existingReservation.Status = reservation.Status;
        existingReservation.TotalPrice = reservation.TotalPrice;

        // Save the changes
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
