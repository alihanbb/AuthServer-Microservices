using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Authserver.Application.DTOs;
using Authserver.Application.Interfaces;
using AuthServerDomain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Authserver.Infrıastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IConfiguration configuration,
         
            ILogger<TokenService> logger)
        {
            _configuration = configuration;

            _logger = logger;
        }

        public RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpirationDate = DateTime.UtcNow.AddMinutes(10)
            };
        }

        public async Task<TokenDto> GenerateToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty)
            };

            // Rolleri token'a ekle
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

          

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            var refreshToken = GenerateRefreshToken();

            return new TokenDto(
                new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken.Token,
                DateTime.UtcNow.AddMinutes(15),    
                DateTime.UtcNow.AddDays(7)           
            );
        }
    }
}
