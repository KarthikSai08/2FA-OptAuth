using Application.Dtos;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await service.RegisterAsync(request);
            return result.Success ? StatusCode(201, result) : Conflict(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            var result = await service.LoginAsync(req);
            return result.Success ? Ok(result) : Unauthorized(result);
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            var result = await service.VerifyOtpAsync(req);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest req)
        {
            var result = await service.ResendOtpAsync(req);
            return Ok(result);
        }
    }
}
