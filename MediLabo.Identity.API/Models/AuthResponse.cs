namespace MediLabo.Identity.API.Models
{
    public class AuthResponse
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
