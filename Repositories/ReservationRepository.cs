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
    public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId)
    {
        return await _context.Reservations
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
    public async Task<bool> CancelReservationAsync(int reservationId, int userId)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

        if (reservation == null)
        {
            throw new InvalidOperationException("Reservation not found.");
        }

        if (reservation.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to cancel this reservation.");
        }

        // Set the status to 'Canceled' or perform your cancellation logic
        reservation.Status = "Canceled";
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<Reservation> GetCompletedReservationAsync(int userId, int carId)
    {
        return await _context.Reservations
            .Where(r => r.UserId == userId && r.CarId == carId
                        && r.DropoffDate < DateTime.Now)  
            .FirstOrDefaultAsync();
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

   
    public async Task<Reservation> AddReservationAsync(Reservation reservation)
    {
        if (string.IsNullOrEmpty(reservation.Status))
        {
            reservation.Status = "pending"; // Default status is set to "pending"
        }

        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
        return reservation;  // Return the saved reservation
    }


    public async Task UpdateReservationAsync(Reservation reservation)
    {
        var existingReservation = await _context.Reservations
            .Include(r => r.Extras)  // Include related CarExtras to handle updates
            .FirstOrDefaultAsync(r => r.ReservationId == reservation.ReservationId);

        if (existingReservation == null)
        {
            throw new Exception("Reservation not found.");
        }

        // Update reservation fields
        existingReservation.DropoffDate = reservation.DropoffDate;

        // Update the CarExtras (if needed)
        existingReservation.Extras.Clear();  // Remove all existing extras (or modify based on logic)

        foreach (var extraId in reservation.CarExtraIds)
        {
            var carExtra = await _context.CarExtras.FindAsync(extraId);
            if (carExtra != null)
            {
                existingReservation.Extras.Add(carExtra);  // Add the selected extra
            }
        }

        // Save changes to the database
        await _context.SaveChangesAsync();
    }
    public async Task<bool> UpdateReservationStatusAsync(int reservationId, string status)
    {
        var existingReservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

        if (existingReservation == null)
        {
            return false; // Reservation not found
        }

        // Update only the status
        existingReservation.Status = status;

        // Save changes
        await _context.SaveChangesAsync();
        return true;
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
