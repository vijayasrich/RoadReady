using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        //Task AddReservationAsync(Reservation reservation);
        Task<Reservation> AddReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId);
        Task<Reservation> GetCompletedReservationAsync(int userId, int carId);
        Task<bool> UpdateReservationStatusAsync(int reservationId, string status);
        Task<bool> CancelReservationAsync(int reservationId, int userId);
        //Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId);

    }
}
