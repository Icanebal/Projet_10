using MediLabo.Identity.API.Services;
using MediLabo.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo.Identity.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<ActionResult<Result<object>>> GetAllUsers()
        {
            var result = await _usersService.GetAllUsersAsync();

            if (result.IsFailure)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteUser(string id)
        {
            var result = await _usersService.DeleteUserAsync(id);

            if (result.IsFailure)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
