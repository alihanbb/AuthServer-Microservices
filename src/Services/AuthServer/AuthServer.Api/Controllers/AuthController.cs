using AuthServer.Application.DTOs;
using AuthServer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Api.Controllers;

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
    public async Task<IActionResult> Register(UserRegisterDto registerDto)
    {
        await _authService.RegisterAsync(registerDto);
        return Ok(new { message = "Kullanıcı başarıyla kaydedildi" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);
        return Ok(token);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var token = await _authService.RefreshTokenAsync(refreshToken);
        return Ok(token);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RevokeRequestDto revokeDto)
    {
        await _authService.RevokeTokenAsync(revokeDto);
        return Ok(new { message = "Token başarıyla iptal edildi" });
    }
}
