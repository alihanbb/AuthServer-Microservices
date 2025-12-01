using AuthServer.Application.DTOs;
using AuthServer.Application.Interfaces;
using AuthServer.Domain.Entities;
using AuthServer.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AuthService(
        IUserService userService,
        ITokenService tokenService,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        _userService = userService;
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<TokenDto> LoginAsync(LoginRequestDto loginDto)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

        if (user == null)
            throw new UserNotFoundException(loginDto.Username);

        var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!passwordValid)
            throw new UnauthorizedAccessException();

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.GenerateToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        refreshToken.UserId = user.Id;
        user.RefreshTokens.Add(refreshToken);
        await _userService.UpdateUserAsync(user);

        return new TokenDto(
            token.AccessToken,
            refreshToken.Token,
            token.AccessTokenExpiration,
            refreshToken.ExpirationDate
        );
    }

    public async Task RegisterAsync(UserRegisterDto registerDto)
    {
        var user = new AppUser
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            UserName = registerDto.Username,
            Email = registerDto.Email,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new DomainInvalidOperationException($"Kullanıcı oluşturulurken hata oluştu: {errors}");
        }

        string roleName = !string.IsNullOrEmpty(registerDto.RoleName) ? registerDto.RoleName : "User";
        
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            roleName = "User";
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new AppRole { Name = roleName });
        }

        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userService.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
            throw new InvalidOperationException("Geçersiz refresh token");

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
        if (userRefreshToken == null || userRefreshToken.ExpirationDate < DateTime.UtcNow)
            throw new InvalidOperationException("Refresh token süresi dolmuş");

        user.RefreshTokens.Remove(userRefreshToken);
        
        var roles = await _userManager.GetRolesAsync(user);
        var newToken = await _tokenService.GenerateToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        newRefreshToken.UserId = user.Id;
        user.RefreshTokens.Add(newRefreshToken);
        await _userService.UpdateUserAsync(user);

        return new TokenDto(
            newToken.AccessToken,
            newRefreshToken.Token,
            newToken.AccessTokenExpiration,
            newRefreshToken.ExpirationDate
        );
    }

    public async Task RevokeTokenAsync(RevokeRequestDto revokeRequestDto)
    {
        var user = await _userService.GetByUsernameAsync(revokeRequestDto.UserName);
        if (user == null)
            throw new UserNotFoundException(revokeRequestDto.UserName);

        var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == revokeRequestDto.RefreshToken);
        if (refreshToken == null)
            throw new InvalidOperationException("Token bulunamadı");

        user.RefreshTokens.Remove(refreshToken);
        await _userService.UpdateUserAsync(user);
    }
}
