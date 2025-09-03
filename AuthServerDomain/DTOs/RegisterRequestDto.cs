using Authserver.Application.DTOs;

public sealed record UserRegisterDto(
        string FirstName,
        string LastName,
        string Username,
        string Email,
        string Password,
        string? RoleName = null); // Opsiyonel rol parametresi ekledik
