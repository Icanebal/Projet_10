using MediLabo.Common;
using MediLabo.Web.Models.ViewModels;

namespace MediLabo.Web.Services;
public class UserService
{
    private readonly ApiService _apiService;
    private readonly ILogger<UserService> _logger;
    private const string UsersEndpoint = "/api/users";

    public UserService(ApiService apiService, ILogger<UserService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<Result<List<UserViewModel>>> GetAllUsersAsync()
    {
        _logger.LogInformation("Attempting to retrieve all users");

        var result = await _apiService.GetAsync<List<UserViewModel>>(UsersEndpoint);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve all users: {Error}", result.Error);
            return result;
        }

        _logger.LogInformation("Successfully retrieved {UserCount} users", result.Value?.Count ?? 0);
        return result;
    }

    public async Task<Result<bool>> DeleteUserAsync(string userId)
    {
        _logger.LogInformation("Attempting to delete user with ID {UserId}", userId);

        var result = await _apiService.DeleteAsync($"{UsersEndpoint}/{userId}");

        if (result.IsFailure)
        {
            _logger.LogError("Failed to delete user with ID {UserId}: {Error}", userId, result.Error);
            return result;
        }

        _logger.LogInformation("Successfully deleted user with ID {UserId}", userId);
        return result;
    }
}
