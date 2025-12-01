namespace AuthServer.Application.DTOs;

public record TokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration
);

public record LoginRequestDto(
    string Username,
    string Password
);

public record UserRegisterDto(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? RoleName = null
);

public record RevokeRequestDto(
    string UserName,
    string RefreshToken
);
