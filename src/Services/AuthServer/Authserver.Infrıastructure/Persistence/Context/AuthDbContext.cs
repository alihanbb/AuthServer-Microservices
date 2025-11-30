using System.Reflection;
using AuthServer.Domain.Entities;
using AuthServerDomain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authserver.Infrıastructure.Persistence.Context
{
    public class AuthDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppRole> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
            
            // OpenIddict entity configurations
            modelBuilder.UseOpenIddict();
        }
    }
}
