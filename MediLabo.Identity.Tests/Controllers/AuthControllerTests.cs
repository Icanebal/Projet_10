using MediLabo.Common;
using MediLabo.Identity.API.Controllers;
using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;
using MediLabo.Common.DTOs;

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
            var registerModel = new RegisterModel
            {
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            var authResponse = new AuthResponseDto
            {
                Id = "user-id",
                Token = "fake-token",
                Email = registerModel.Email,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Roles = new List<string> { "User" }
            };

            _authServiceMock.Setup(x => x.RegisterAsync(registerModel))
                .ReturnsAsync(Result<AuthResponseDto>.Success(authResponse));

            var actionResult = await _controller.Register(registerModel);

            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var resultValue = okResult.Value.Should().BeOfType<Result<AuthResponseDto>>().Subject;
            resultValue.IsSuccess.Should().BeTrue();
            resultValue.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task Register_WithInvalidModel_ShouldReturnBadRequest()
        {
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

            var actionResult = await _controller.Register(registerModel);

            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_WhenServiceFails_ShouldReturnBadRequest()
        {
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
                .ReturnsAsync(Result<AuthResponseDto>.Failure("Email already exists"));

            var actionResult = await _controller.Register(registerModel);

            var badRequestResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var resultValue = badRequestResult.Value.Should().BeOfType<Result<AuthResponseDto>>().Subject;
            resultValue.IsFailure.Should().BeTrue();
            resultValue.Error.Should().Contain("Email already exists");
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            var loginModel = new LoginModel
            {
                Email = "test@test.com",
                Password = "Test123!"
            };

            var authResponse = new AuthResponseDto
            {
                Id = "user-id",
                Token = "fake-token",
                Email = loginModel.Email,
                FirstName = "Test",
                LastName = "User",
                Roles = new List<string> { "User" }
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(Result<AuthResponseDto>.Success(authResponse));

            var actionResult = await _controller.Login(loginModel);

            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var resultValue = okResult.Value.Should().BeOfType<Result<AuthResponseDto>>().Subject;
            resultValue.IsSuccess.Should().BeTrue();
            resultValue.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task Login_WithInvalidModel_ShouldReturnBadRequest()
        {
            var loginModel = new LoginModel
            {
                Email = "test@test.com",
                Password = "Test123!"
            };

            _controller.ModelState.AddModelError("Email", "Email is required");

            var actionResult = await _controller.Login(loginModel);

            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var loginModel = new LoginModel
            {
                Email = "test@test.com",
                Password = "WrongPassword!"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginModel))
                .ReturnsAsync(Result<AuthResponseDto>.Failure("Invalid email or password"));

            var actionResult = await _controller.Login(loginModel);

            var unauthorizedResult = actionResult.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            var resultValue = unauthorizedResult.Value.Should().BeOfType<Result<AuthResponseDto>>().Subject;
            resultValue.IsFailure.Should().BeTrue();
            resultValue.Error.Should().Contain("Invalid email or password");
        }
    }
}
