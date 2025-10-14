namespace MediLabo.Identity.API.Models.DTOs
{
    public class UserDto
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public IList<string> Roles { get; init; } = new List<string>();
        public DateTime CreatedAt { get; init; }
    }
}