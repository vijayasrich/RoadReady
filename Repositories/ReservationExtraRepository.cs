/*using RoadReady.Authentication;
using RoadReady.Models;

namespace RoadReady.Repositories
{
    public class ReservationExtraRepository: IReservationExtraRepository
    {
        private readonly RoadReadyContext _context;

        public ReservationExtraRepository(RoadReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<ReservationExtra> GetAllReservationExtras()
        {
            return _context.ReservationExtras.ToList();
        }

        public void AddReservationExtra(ReservationExtra reservationExtra)
        {
            _context.ReservationExtras.Add(reservationExtra);
            _context.SaveChanges();
        }


        public void DeleteReservationExtra(int reservationId, int extraId)
        {
            var reservationExtra = _context.ReservationExtras
                .FirstOrDefault(re => re.ReservationId == reservationId && re.ExtraId == extraId);
            if (reservationExtra != null)
            {
                _context.ReservationExtras.Remove(reservationExtra);
                _context.SaveChanges();
            }
        }
    }
}*/
    
