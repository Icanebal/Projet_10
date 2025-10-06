using MediLabo.Identity.API.Models;

namespace MediLabo.Identity.API.Services
{
    public interface ITokenService
    {
        Task<AuthResponse> GenerateTokenAsync(ApplicationUser user);
    }
}