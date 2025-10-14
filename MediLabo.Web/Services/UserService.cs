using MediLabo.Common;
using MediLabo.Web.Models.ViewModels;

namespace MediLabo.Web.Services;

public class UserService
{
    private readonly IApiService _apiService;
    private readonly ILogger<UserService> _logger;
    private const string UsersEndpoint = "/api/users";

    public UserService(IApiService apiService, ILogger<UserService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Récupère tous les utilisateurs
    public async Task<Result<List<UserViewModel>>> GetAllUsersAsync()
    {
        _logger.LogInformation("Retrieving all users");
        var result = await _apiService.GetAsync<List<UserViewModel>>(UsersEndpoint);

        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Get all users: {IsSuccess}, Count: {Count}, Error: {Error}",
            result.IsSuccess, result.Value?.Count, result.Error);

        return result;
    }

    // Supprime un utilisateur
    public async Task<Result<bool>> DeleteUserAsync(string userId)
    {
        _logger.LogInformation("Deleting user with ID {UserId}", userId);
        var result = await _apiService.DeleteAsync($"{UsersEndpoint}/{userId}");

        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Delete user {UserId}: {IsSuccess}, Error: {Error}",
            userId, result.IsSuccess, result.Error);

        return result;
    }
}
