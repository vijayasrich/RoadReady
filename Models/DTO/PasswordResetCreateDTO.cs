namespace RoadReady.Models.DTO
{
    public class PasswordResetCreateDTO
    {
        public int UserId { get; set; }

        
        public string Token { get; set; } = null!;
    }
}
