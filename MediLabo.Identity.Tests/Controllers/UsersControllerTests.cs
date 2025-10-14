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
            // Arrange
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

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUsers = okResult.Value.Should().BeAssignableTo<IEnumerable<UserDto>>().Subject;
            returnedUsers.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllUsers_WhenServiceFails_ShouldReturnBadRequest()
        {
            // Arrange
            _usersServiceMock.Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(Result<IEnumerable<UserDto>>.Failure("Database error"));

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteUser_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var userId = "test-user-id";

            _usersServiceMock.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(Result<bool>.Success(true));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _usersServiceMock.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = "non-existent-id";

            _usersServiceMock.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(Result<bool>.Failure("User not found"));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteUser_WhenDeletingDefaultAdmin_ShouldReturnNotFound()
        {
            // Arrange
            var userId = "admin-id";

            _usersServiceMock.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(Result<bool>.Failure("Cannot delete the default admin account"));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().NotBeNull();
        }
    }
}
