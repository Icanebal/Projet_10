using MediLabo.Common;
using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MediLabo.Identity.API.Services
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersService> _logger;

        public UsersService(
            UserManager<ApplicationUser> userManager,
            ILogger<UsersService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userManager.Users.ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles,
                    CreatedAt = user.CreatedAt
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} users", userDtos.Count);

            return Result<IEnumerable<UserDto>>.Success(userDtos);
        }

        public async Task<Result<bool>> DeleteUserAsync(string userId)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return Result<bool>.Failure("User not found");
            }

            if (user.Email == "admin@medilabo.com")
            {
                _logger.LogWarning("Attempted to delete default admin account");
                return Result<bool>.Failure("Cannot delete the default admin account");
            }

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to delete user {UserId}. Errors: {Errors}", userId, errors);
                return Result<bool>.Failure($"Failed to delete user: {errors}");
            }

            _logger.LogInformation("User {UserId} ({Email}) deleted successfully", userId, user.Email);

            return Result<bool>.Success(true);
        }
    }
}
