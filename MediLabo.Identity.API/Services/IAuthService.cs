using MediLabo.Identity.API.Models;
using MediLabo.Common;

namespace MediLabo.Identity.API.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterModel model);

        Task<Result<AuthResponseDto>> LoginAsync(LoginModel model);
    }
}
