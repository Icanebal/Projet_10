using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error, errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(model);

            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.Error });
            }

            return Ok(result.Value);
        }
    }
}
