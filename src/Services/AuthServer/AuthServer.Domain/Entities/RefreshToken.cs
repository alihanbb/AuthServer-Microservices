namespace AuthServer.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
