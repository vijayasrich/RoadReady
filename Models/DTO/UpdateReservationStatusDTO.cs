namespace RoadReady.Models.DTO
{
    public class UpdateReservationStatusDTO
    {
        public int ReservationId { get; set; }
        public string Status { get; set; } // e.g., "confirmed", "cancelled"
    }

}
