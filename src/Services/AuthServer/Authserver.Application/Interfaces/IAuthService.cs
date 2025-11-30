using Authserver.Application.DTOs;

namespace Authserver.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginRequestDto loginDto);
        Task RegisterAsync(UserRegisterDto registerDto);
        Task<TokenDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(RevokeRequestDto revokeRequestDto);
    }
}
