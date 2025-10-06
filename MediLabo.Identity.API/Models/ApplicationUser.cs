using Microsoft.AspNetCore.Identity;

namespace MediLabo.Identity.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
