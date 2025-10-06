using MediLabo.Identity.API.Models;
using MediLabo.Common;

namespace MediLabo.Identity.API.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterModel model);

        Task<Result<AuthResponse>> LoginAsync(LoginModel model);
    }
}
