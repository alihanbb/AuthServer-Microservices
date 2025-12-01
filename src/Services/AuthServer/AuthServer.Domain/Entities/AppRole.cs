using Microsoft.AspNetCore.Identity;

namespace AuthServer.Domain.Entities;

public class AppRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
