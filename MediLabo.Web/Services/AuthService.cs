using MediLabo.Common;
using MediLabo.Common.DTOs;

namespace MediLabo.Web.Services;

public class AuthService
{
    private readonly IApiService _apiService;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string AuthEndpoint = "/api/auth";
    private const string TokenSessionKey = "JwtToken";
    private const string UserNameSessionKey = "UserName";
    private const string UserRoleSessionKey = "UserRole";

    public AuthService(
        IApiService apiService,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _apiService = apiService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<string>> LoginAsync(string username, string password)
    {
        _logger.LogInformation("Login attempt for user {Username}", username);

        var loginRequest = new { email = username, password };
        var result = await _apiService.PostAsync<object, AuthResponseDto>($"{AuthEndpoint}/login", loginRequest);

        if (result.IsSuccess && result.Value != null)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(TokenSessionKey, result.Value.Token);
                session.SetString(UserNameSessionKey, username);

                if (result.Value.Roles != null && result.Value.Roles.Any())
                {
                    var role = result.Value.Roles.First();
                    session.SetString(UserRoleSessionKey, role);
                    _logger.LogInformation("User {Username} logged in successfully with role {Role}", username, role);
                }
                else
                {
                    _logger.LogInformation("User {Username} logged in successfully", username);
                }
            }

            return Result<string>.Success(result.Value.Token);
        }

        _logger.LogError("Login failed for user {Username}: {Error}", username, result.Error);
        return Result<string>.Failure(result.Error ?? "Unknown error");
    }

    public void Logout()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        var username = session?.GetString(UserNameSessionKey);

        _logger.LogInformation("User {Username} logging out", username ?? "Unknown");

        session?.Remove(TokenSessionKey);
        session?.Remove(UserNameSessionKey);
        session?.Remove(UserRoleSessionKey);
    }

    public async Task<Result<string>> RegisterAsync(string email, string password, string firstName, string lastName, string role)
    {
        _logger.LogInformation("Registering new user with email {Email}", email);

        var registerRequest = new
        {
            email,
            password,
            confirmPassword = password,
            firstName,
            lastName,
            role
        };

        var result = await _apiService.PostAsync<object, AuthResponseDto>($"{AuthEndpoint}/register", registerRequest);

        if (result.IsSuccess && result.Value != null)
        {
            _logger.LogInformation("User {Email} registered successfully", email);
            return Result<string>.Success($"Utilisateur {result.Value.FirstName} {result.Value.LastName} créé avec succès");
        }

        _logger.LogError("Registration failed for email {Email}: {Error}", email, result.Error);
        return Result<string>.Failure(result.Error ?? "Unknown error");
    }

    public string? GetToken()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey);
    }

    public string? GetUserName()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString(UserNameSessionKey);
    }

    public string? GetUserRole()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString(UserRoleSessionKey);
    }

    public bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(GetToken());
    }

    public bool IsAdmin()
    {
        var role = GetUserRole();
        return role != null && role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }
}
