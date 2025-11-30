using AuthServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthServerDomain.Entities;
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } 
    }

