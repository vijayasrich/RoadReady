namespace RoadReady.Models
{
    public class PasswordResetDTO
    {
        public int ResetId { get; set; }
        public int? UserId { get; set; }
        public string? Token { get; set; }
        public UserDTO? User { get; set; } // Related User information
    }

    // DTO for the User model (assuming User class exists)
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } // or any other fields relevant for the User
        public string Email { get; set; }
        // Include other fields from the User model as needed
    }
}
