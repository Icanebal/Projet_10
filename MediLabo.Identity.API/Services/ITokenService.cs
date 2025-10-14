using MediLabo.Identity.API.Models;
using MediLabo.Common;

namespace MediLabo.Identity.API.Services
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user);
    }
}