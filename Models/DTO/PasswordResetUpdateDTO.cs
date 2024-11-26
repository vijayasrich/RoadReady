namespace RoadReady.Models.DTO
{
    public class PasswordResetUpdateDTO
    {
        public int ResetId { get; set; }

       
        public string Token { get; set; } = null!;
    }
}
