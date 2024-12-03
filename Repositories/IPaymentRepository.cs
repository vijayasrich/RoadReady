﻿using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task AddPaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task DeletePaymentAsync(int id);
        //Task<IEnumerable<Payment>> GetPaymentsByReservationIdsAsync(IEnumerable<int> reservationIds);
        Task<List<Payment>> GetPaymentsByUserIdAsync(int userId);
    }
}
