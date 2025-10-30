namespace MediLabo.Common.DTOs
{
    public class AuthResponseDto
    {
        public required string Id { get; set; }
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
