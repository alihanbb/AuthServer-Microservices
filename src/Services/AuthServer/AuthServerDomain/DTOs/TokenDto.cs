namespace Authserver.Application.DTOs;


public sealed record TokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration);
