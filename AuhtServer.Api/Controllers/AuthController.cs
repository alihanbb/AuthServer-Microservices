using Authserver.Application.DTOs;
using Authserver.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuhtServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            await authService.RegisterAsync(userRegisterDto);
            return Ok("Kullanıcı kaydı başarılı");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto userLoginDto)
        {
            var loginUser = await authService.LoginAsync(userLoginDto);
            return Ok(loginUser);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var token = await authService.RefreshTokenAsync(refreshToken);
            return Ok(token);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(RevokeRequestDto revokeRequestDto)
        {
            
            await authService.RevokeTokenAsync(revokeRequestDto);
            return Ok("Refresh token başarıyla iptal edildi.");
        }
    }
}
