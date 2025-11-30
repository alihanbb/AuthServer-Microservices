namespace AuthServerDomain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public Guid UserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
