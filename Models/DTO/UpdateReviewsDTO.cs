﻿namespace RoadReady.Models.DTO
{
    public class UpdateReviewsDTO
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
    }
}
