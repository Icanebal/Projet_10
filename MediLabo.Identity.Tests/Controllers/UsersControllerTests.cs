using MediLabo.Common;
using MediLabo.Identity.API.Controllers;
using MediLabo.Identity.API.Models.DTOs;
using MediLabo.Identity.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace MediLabo.Identity.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersService> _usersServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _usersServiceMock = new Mock<IUsersService>();
            _controller = new UsersController(_usersServiceMock.Object);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkWithUsersList()
        {
            var users = new List<UserDto>
            {
                new UserDto
                {
                    Id = "1",
                    Email = "user1@test.com",
                    FirstName = "User",
                    LastName = "One",
                    Roles = new List<string> { "User" },
                    CreatedAt = DateTime.UtcNow
                },
                new UserDto
                {
                    Id = "2",
                    Email = "user2@test.com",
                    FirstName = "User",
                    LastName = "Two",
                    Roles = new List<string> { "Admin" },
                    CreatedAt = DateTime.UtcNow
                }
            };

            _usersServiceMock.Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(Result<IEnumerable<UserDto>>.Success(users));

            var actionResult = await _controller.GetAllUsers();

            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var resultValue = okResult.Value.Should().BeOfType<Result<IEnumerable<UserDto>>>().Subject;
            resultValue.IsSuccess.Should().BeTrue();
            resultValue.Value.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllUsers_WhenServiceFails_ShouldReturnBadRequest()
        {
            _usersServiceMock.Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(Result<IEnumerable<UserDto>>.Failure("Database error"));

            var actionResult = await _controller.GetAllUsers();

            var badRequestResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var resultValue = badRequestResult.Value.Should().BeOfType<Result<IEnumerable<UserDto>>>().Subject;
            resultValue.IsFailure.Should().BeTrue();
            resultValue.Error.Should().Contain("Database error");
        }

        [Fact]
        public async Task DeleteUser_WithValidId_ShouldReturnOk()
        {
            var userId = "test-user-id";

            _usersServiceMock.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(Result<bool>.Success(true));

            var actionResult = await _controller.DeleteUser(userId);

            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var resultValue = okResult.Value.Should().BeOfType<Result<bool>>().Subject;
            resultValue.IsSuccess.Should().BeTrue();
            resultValue.Value.Should().BeTrue();
            _usersServiceMock.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithNonExistentId_ShouldReturnNotFound()
        {
            var userId = "non-existent-id";

            _usersServiceMock.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(Result<bool>.Failure("User not found"));

            var actionResult = await _controller.DeleteUser(userId);

            var notFoundResult = actionResult.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            var resultValue = notFoundResult.Value.Should().BeOfType<Result<bool>>().Subject;
            resultValue.IsFailure.Should().BeTrue();
            resultValue.Error.Should().Contain("User not found");
        }

        [Fact]
        public async Task DeleteUser_WhenDeletingDefaultAdmin_ShouldReturnNotFound()
        {
            var userId = "admin-id";

            _usersServiceMock.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(Result<bool>.Failure("Cannot delete the default admin account"));

            var actionResult = await _controller.DeleteUser(userId);

            var notFoundResult = actionResult.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            var resultValue = notFoundResult.Value.Should().BeOfType<Result<bool>>().Subject;
            resultValue.IsFailure.Should().BeTrue();
            resultValue.Error.Should().Contain("Cannot delete the default admin account");
        }
    }
}
