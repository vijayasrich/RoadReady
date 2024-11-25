using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IReservationExtraRepository
    {
        IEnumerable<ReservationExtra> GetAllReservationExtras();
        void AddReservationExtra(ReservationExtra reservationExtra);
        void DeleteReservationExtra(int reservationId, int extraId);
    }
}
