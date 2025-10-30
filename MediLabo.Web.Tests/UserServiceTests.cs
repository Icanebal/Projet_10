using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MediLabo.Web.Services;
using MediLabo.Web.Models.ViewModels;
using MediLabo.Common;
using MediLabo.Common.HttpServices;

namespace MediLabo.Web.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockApiService = new Mock<IApiService>();

        _mockLogger = new Mock<ILogger<UserService>>();

        _userService = new UserService(
            _mockApiService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsSuccessWithUsers()
    {
        // Arrange
        var users = new List<UserViewModel>
        {
            new() { Id = "1", Email = "user1@test.com", FirstName = "User", LastName = "One", Roles = new List<string> { "User" } },
            new() { Id = "2", Email = "admin@test.com", FirstName = "Admin", LastName = "User", Roles = new List<string> { "Admin" } }
        };

        _mockApiService
            .Setup(s => s.GetAsync<List<UserViewModel>>(It.IsAny<string>()))
            .ReturnsAsync(Result<List<UserViewModel>>.Success(users));

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(u => u.Email == "admin@test.com");
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenApiFails_ReturnsFailure()
    {
        // Arrange
        _mockApiService
            .Setup(s => s.GetAsync<List<UserViewModel>>(It.IsAny<string>()))
            .ReturnsAsync(Result<List<UserViewModel>>.Failure("API Error"));

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("API Error");
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var userId = "user-123";

        _mockApiService
            .Setup(s => s.DeleteAsync($"/api/users/{userId}"))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUserAsync_WhenApiFails_ReturnsFailure()
    {
        // Arrange
        var userId = "user-123";

        _mockApiService
            .Setup(s => s.DeleteAsync($"/api/users/{userId}"))
            .ReturnsAsync(Result<bool>.Failure("User not found"));

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("User not found");
    }
}
