namespace RoadReady.Models.DTO
{
    public class ReviewRequestDTO
    {
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
    }
}
