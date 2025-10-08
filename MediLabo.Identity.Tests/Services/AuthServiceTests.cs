using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediLabo.Identity.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                contextAccessor.Object,
                userPrincipalFactory.Object,
                null, null, null, null);

            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Register_ShouldSucceed_WhenValidData()
        {
            var model = new RegisterModel { Email = "test@test.com", Password = "StrongPass123!", Role = "User", ConfirmPassword = "StrongPass123!", FirstName = "John", LastName = "Doe"};
            var user = new ApplicationUser { Email = model.Email, FirstName = "John", LastName = "Doe"};

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), model.Role))
                .ReturnsAsync(IdentityResult.Success);

            var authResponse = new AuthResponse { Token = "fake-token", Email = model.Email, FirstName = "John", LastName = "Doe"};
            _tokenServiceMock.Setup(t => t.GenerateTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(authResponse);

            var result = await _authService.RegisterAsync(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("fake-token", result.Value.Token);
        }

        [Fact]
        public async Task Register_ShouldFail_WhenEmailAlreadyExists()
        {
            var model = new RegisterModel { Email = "test@test.com", Password = "StrongPass123!", Role = "User", ConfirmPassword = "StrongPass123!", FirstName = "John", LastName = "Doe" };
            var existingUser = new ApplicationUser { Email = model.Email, FirstName = "John", LastName = "Doe" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync(existingUser);

            var result = await _authService.RegisterAsync(model);

            Assert.False(result.IsSuccess);
            Assert.Contains("Email already exists", result.Errors);
        }

        [Fact]
        public async Task Register_ShouldFail_WhenPasswordTooWeak()
        {
            var model = new RegisterModel { Email = "test@test.com", Password = "WeakPass", Role = "User", ConfirmPassword = "StrongPass123!", FirstName = "John", LastName = "Doe" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

            var result = await _authService.RegisterAsync(model);

            Assert.False(result.IsSuccess);
            Assert.Contains("Password too weak", result.Errors);
        }

        [Fact]
        public async Task Register_ShouldAssignDefaultRole_WhenNoRoleSpecified()
        {
            var model = new RegisterModel { Email = "test@test.com", Password = "StrongPass123!", ConfirmPassword = "StrongPass123!", FirstName = "John", LastName = "Doe" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            _tokenServiceMock.Setup(t => t.GenerateTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new AuthResponse { Token = "default-token", Email = model.Email, FirstName = "John", LastName = "Doe" });

            var result = await _authService.RegisterAsync(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("default-token", result.Value.Token);
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldSucceed_WhenCredentialsAreValid()
        {
            var model = new LoginModel { Email = "test@test.com", Password = "StrongPass123!" };
            var user = new ApplicationUser { Email = model.Email, FirstName = "John", LastName = "Doe" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, model.Password, false))
                .ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(t => t.GenerateTokenAsync(user))
                .ReturnsAsync(new AuthResponse { Token = "login-token", Email = model.Email, FirstName = "John", LastName = "Doe" });

            var result = await _authService.LoginAsync(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("login-token", result.Value.Token);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenEmailDoesNotExist()
        {
            var model = new LoginModel { Email = "notfound@test.com", Password = "StrongPass123!" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);

            var result = await _authService.LoginAsync(model);

            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid email or password", result.Errors);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenPasswordIsIncorrect()
        {
            var model = new LoginModel { Email = "test@test.com", Password = "WrongPass" };
            var user = new ApplicationUser { Email = model.Email, FirstName = "John", LastName = "Doe" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, model.Password, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _authService.LoginAsync(model);

            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid email or password", result.Errors);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenSuccessful()
        {
            var model = new LoginModel { Email = "test@test.com", Password = "StrongPass123!" };
            var user = new ApplicationUser { Email = model.Email, FirstName = "John", LastName = "Doe" };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, model.Password, false))
                .ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(t => t.GenerateTokenAsync(user))
                .ReturnsAsync(new AuthResponse { Token = "jwt-token", Email = model.Email, FirstName = "John", LastName = "Doe" });

            var result = await _authService.LoginAsync(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("jwt-token", result.Value.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnCorrectRoles_InAuthResponse()
        {
            var model = new LoginModel { Email = "role@test.com", Password = "StrongPass123!" };
            var user = new ApplicationUser { Email = model.Email, FirstName = "John", LastName = "Doe"};

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, model.Password, false))
                .ReturnsAsync(SignInResult.Success);

            var authResponse = new AuthResponse { Token = "fake-token", Email = model.Email, FirstName = "John", LastName = "Doe", Roles = new List<string> { "Admin", "User" } };
        _tokenServiceMock.Setup(t => t.GenerateTokenAsync(user)).ReturnsAsync(authResponse);

            var result = await _authService.LoginAsync(model);

            Assert.True(result.IsSuccess);
            Assert.Contains("Admin", result.Value.Roles);
            Assert.Contains("User", result.Value.Roles);
        }
    }
}

