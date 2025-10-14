using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Services;
using Microsoft.AspNetCore.Mvc;
using MediLabo.Common;

namespace MediLabo.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Result<object>>> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Result<object>.Failure("Invalid model state"));
            }

            var result = await _authService.RegisterAsync(model);

            if (result.IsFailure)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result<object>>> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Result<object>.Failure("Invalid model state"));
            }

            var result = await _authService.LoginAsync(model);

            if (result.IsFailure)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
