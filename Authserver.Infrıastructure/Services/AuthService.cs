using Authserver.Application.DTOs;
using Authserver.Application.Interfaces;
using AuthServer.Domain.Entities;
using AuthServer.Domain.Exceptions;
using AuthServerDomain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authserver.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher passwordHasher;
        private readonly IUserService userService;
        private readonly ITokenService tokenService;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;

        public AuthService(
            IPasswordHasher passwordHasher,
            IUserService userService,
            ITokenService tokenService,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            this.passwordHasher = passwordHasher;
            this.userService = userService;
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<TokenDto> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
            
            if (user == null) 
                throw new UserNotFoundException(loginDto.Username);

            var passwordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
                throw new UnauthorizedAccessException();

            // Kullanıcının rollerini al
            var roles = await userManager.GetRolesAsync(user);
            
            // Token üret (async olarak)
            var token = await tokenService.GenerateToken(user, roles);
            var refreshToken = tokenService.GenerateRefreshToken();
            
            user.RefreshTokens.Add(refreshToken);
            await userService.UpdateUserAsync(user);

            return new TokenDto(
                token.AccessToken,
                refreshToken.Token,
                token.AccessTokenExpiration,
                refreshToken.ExpirationDate
            );
        }

        public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
            if (user == null)
                throw new InvalidCreateException();

            var userRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
            if (userRefreshToken == null || userRefreshToken.ExpirationDate < DateTime.UtcNow)
                throw new InvalidCreateException();

            user.RefreshTokens.Remove(userRefreshToken);

            // Kullanıcının rollerini al
            var roles = await userManager.GetRolesAsync(user);
            
            // Token üret (async olarak)
            var newToken = await tokenService.GenerateToken(user, roles);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            await userService.UpdateUserAsync(user);

            return new TokenDto(
                newToken.AccessToken,
                newRefreshToken.Token,
                newToken.AccessTokenExpiration,
                newRefreshToken.ExpirationDate
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
            
            var result = await userManager.CreateAsync(user, registerDto.Password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Kullanıcı oluşturulurken hata oluştu: {errors}");
            }

            // Rol atama işlemi - "User" rolü olarak düzeltildi ("Member" yerine)
            string roleName = !string.IsNullOrEmpty(registerDto.RoleName) ? registerDto.RoleName : "User";
            
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                roleName = "User";
                
             
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
                }
            }
            
            var roleResult = await userManager.AddToRoleAsync(user, roleName);
            
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Kullanıcıya rol atanırken hata oluştu: {errors}");
            }
        }

        public async Task RevokeTokenAsync(RevokeRequestDto revokeRequestDto)
        {
            var user = await userService.GetByUsernameAsync(revokeRequestDto.UserName);
            if (user == null)
                throw new UserNotFoundException(revokeRequestDto.UserName);

            var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == revokeRequestDto.RefreshToken);
            if (refreshToken == null)
                throw new InvalidCreateException();

            user.RefreshTokens.Remove(refreshToken);
        }
    }
}
