using FluentAssertions;
using MediLabo.Common;
using MediLabo.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediLabo.Web.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ISession> _mockSession;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockApiService = new Mock<IApiService>();

        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockSession = new Mock<ISession>();

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        _authService = new AuthService(
            _mockApiService.Object,
            _mockLogger.Object,
            _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var username = "admin@medilabo.com";
        var password = "Admin123!";
        var token = "fake-jwt-token";

        var authResponse = new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            Email = username,
            FirstName = "Admin",
            LastName = "User",
            Roles = new List<string> { "Admin" }
        };

        _mockApiService
            .Setup(s => s.PostAsync<object, AuthResponseDto>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(Result<AuthResponseDto>.Success(authResponse));

        var sessionData = new Dictionary<string, byte[]>();
        _mockSession
            .Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Callback<string, byte[]>((key, value) => sessionData[key] = value);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(token);
        _mockSession.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var username = "invalid@medilabo.com";
        var password = "WrongPassword";

        _mockApiService
            .Setup(s => s.PostAsync<object, AuthResponseDto>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(Result<AuthResponseDto>.Failure("Invalid credentials"));

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid credentials");
    }

    [Fact]
    public void IsAdmin_WithAdminRole_ReturnsTrue()
    {
        // Arrange
        var roleKey = "UserRole";
        var roleValue = System.Text.Encoding.UTF8.GetBytes("Admin");

        _mockSession
            .Setup(s => s.TryGetValue(roleKey, out roleValue))
            .Returns(true);

        // Act
        var isAdmin = _authService.IsAdmin();

        // Assert
        isAdmin.Should().BeTrue();
    }

    [Fact]
    public void IsAdmin_WithUserRole_ReturnsFalse()
    {
        // Arrange
        var roleKey = "UserRole";
        var roleValue = System.Text.Encoding.UTF8.GetBytes("User");

        _mockSession
            .Setup(s => s.TryGetValue(roleKey, out roleValue))
            .Returns(true);

        // Act
        var isAdmin = _authService.IsAdmin();

        // Assert
        isAdmin.Should().BeFalse();
    }

    [Fact]
    public void Logout_RemovesSessionData()
    {
        // Act
        _authService.Logout();

        // Assert
        _mockSession.Verify(s => s.Remove("JwtToken"), Times.Once);
        _mockSession.Verify(s => s.Remove("UserName"), Times.Once);
        _mockSession.Verify(s => s.Remove("UserRole"), Times.Once);
    }

    [Fact]
    public void IsAuthenticated_WithToken_ReturnsTrue()
    {
        // Arrange
        var tokenKey = "JwtToken";
        var tokenValue = System.Text.Encoding.UTF8.GetBytes("fake-token");

        _mockSession
            .Setup(s => s.TryGetValue(tokenKey, out tokenValue))
            .Returns(true);

        // Act
        var isAuthenticated = _authService.IsAuthenticated();

        // Assert
        isAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_WithoutToken_ReturnsFalse()
    {
        // Arrange
        byte[]? nullValue = null;
        _mockSession
            .Setup(s => s.TryGetValue("JwtToken", out nullValue))
            .Returns(false);

        // Act
        var isAuthenticated = _authService.IsAuthenticated();

        // Assert
        isAuthenticated.Should().BeFalse();
    }
}
