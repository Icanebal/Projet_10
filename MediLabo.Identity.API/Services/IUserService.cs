using MediLabo.Common;
using MediLabo.Identity.API.Models.DTOs;

namespace MediLabo.Identity.API.Services
{
    public interface IUsersService
    {
        Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<Result<bool>> DeleteUserAsync(string userId);
    }
}