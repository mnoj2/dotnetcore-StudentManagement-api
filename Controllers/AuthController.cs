using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Entities.Models;
using StudentManagementApi.Services;

namespace StudentManagementApi.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {

        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) { 
            _auth = auth; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto) {

            var user = await _auth.RegisterAsync(dto);

            if(user is null)
                return BadRequest("Username already exists");

            return Ok(new { user.Id, user.Username, user.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto) {

            var token = await _auth.LoginAsync(dto);

            if(token is null)
                return BadRequest("Invalid credentials");

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto) {

            var token = await _auth.RefreshTokensAsync(dto);

            if(token is null)
                return Unauthorized("Invalid refresh token");

            return Ok(token);
        }

    }
}
