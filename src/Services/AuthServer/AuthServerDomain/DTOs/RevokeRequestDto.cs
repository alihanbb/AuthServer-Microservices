namespace Authserver.Application.DTOs;

public sealed record RevokeRequestDto(string RefreshToken, string UserName);

