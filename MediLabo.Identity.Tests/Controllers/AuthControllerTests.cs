using MediLabo.Common;
using MediLabo.Identity.API.Controllers;
using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace MediLabo.Identity.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_WithValidModel_ShouldReturnOk()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            var authResponse = new AuthResponse
            {
                Token = "fake-token",
                Email = registerModel.Email,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Roles = new List<string> { "User" }
            };

            _authServiceMock.Setup(x => x.RegisterAsync(registerModel))
                .ReturnsAsync(Result<AuthResponse>.Success(authResponse));

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task Register_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_WhenServiceFails_ShouldReturnBadRequest()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(registerModel))
                .ReturnsAsync(Result<AuthResponse>.Failure("Email already exists"));

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@test.com",
                Password = "Test123!"
            };

            var authResponse = new AuthResponse
            {
                Token = "fake-token",
                Email = loginModel.Email,
                FirstName = "Test",
                LastName = "User",
                Roles = new List<string> { "User" }
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(Result<AuthResponse>.Success(authResponse));

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task Login_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@test.com",
                Password = "Test123!"
            };

            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@test.com",
                Password = "WrongPassword!"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(Result<AuthResponse>.Failure("Invalid email or password"));

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().NotBeNull();
        }
    }
}