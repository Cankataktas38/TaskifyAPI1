using Microsoft.AspNetCore.Mvc;
using TaskifyAPI.DTOs;
using TaskifyAPI.Services;
namespace TaskifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService? _auth;
        public AuthController(IAuthService? auth)
        {
            _auth = auth;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existing = await _auth.GetUserByName(dto.UserName);
            if (existing != null) return BadRequest("Kullanıcı zaten var");
            var user = await _auth.Register(dto);
            return Ok(new { user.Id,user.UserName });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _auth.Login(dto);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }
    }
}
