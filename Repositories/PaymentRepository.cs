using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PaymentRepository : IPaymentRepository
{
    private readonly RoadReadyContext _context;

    public PaymentRepository(RoadReadyContext context)
    {
        _context = context;
    }
    public async Task<List<Payment>> GetPaymentsByUserIdAsync(int userId)
    {
        try
        {
            // Fetch payments based on userId by joining Reservation and Payment
            var payments = await _context.Payments
                .Join(_context.Reservations, // Join Payments with Reservations
                      payment => payment.ReservationId, // Payment's ReservationId
                      reservation => reservation.ReservationId, // Reservation's ReservationId
                      (payment, reservation) => new { payment, reservation })
                .Where(x => x.reservation.UserId == userId) // Filter by UserId in Reservation
                .Select(x => x.payment) // Select only the Payment object
                .ToListAsync();

            return payments;
        }
        catch (Exception ex)
        {
            // Log or handle exceptions
            throw new Exception("Error fetching payments by userId", ex);
        }
    }
    public async Task<Payment> GetPaymentByIdAsync(int id)
    {
        return await _context.Payments.FindAsync(id);
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        // Check for a tracked instance with the same key
        var trackedEntity = _context.ChangeTracker.Entries<Payment>()
                                    .FirstOrDefault(e => e.Entity.PaymentId == payment.PaymentId);

        if (trackedEntity != null)
        {
            // Detach the tracked instance to avoid conflict
            trackedEntity.State = EntityState.Detached;
        }

        // Attach and update the payment entity
        _context.Payments.Update(payment);

        // Save changes
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Payment>> GetPaymentsByReservationIdsAsync(IEnumerable<int> reservationIds)
    {
        return await _context.Payments
            .Where(payment => reservationIds.Contains(payment.ReservationId))
            .ToListAsync();
    }


    public async Task DeletePaymentAsync(int id)
    {
        var payment = await GetPaymentByIdAsync(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }
}

