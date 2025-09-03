using AuthServerDomain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }
}
