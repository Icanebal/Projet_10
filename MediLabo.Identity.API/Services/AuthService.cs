using MediLabo.Identity.API.Models;
using MediLabo.Common;
using Microsoft.AspNetCore.Identity;

namespace MediLabo.Identity.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }
        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterModel model)
        {
            _logger.LogInformation("Attempting to register user with email: {Email}", model.Email);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", model.Email);
                return Result<AuthResponseDto>.Failure("Email already exists");
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description).ToList();
                _logger.LogError("User creation failed for {Email}. Errors: {Errors}",
                    model.Email, string.Join(", ", errors));
                return Result<AuthResponseDto>.Failure(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Failed to assign role {Role} to user {Email}", model.Role, model.Email);
            }

            _logger.LogInformation("User {Email} registered successfully with role {Role}",
                model.Email, model.Role);

            var authResponse = await _tokenService.GenerateTokenAsync(user);

            return Result<AuthResponseDto>.Success(authResponse);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginModel model)
        {
            _logger.LogInformation("Login attempt for email: {Email}", model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User with email {Email} not found", model.Email);
                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for email {Email}", model.Email);
                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            _logger.LogInformation("User {Email} logged in successfully", model.Email);

            var authResponse = await _tokenService.GenerateTokenAsync(user);

            return Result<AuthResponseDto>.Success(authResponse);
        }
    }
}
