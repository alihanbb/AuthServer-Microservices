namespace AuthServer.Domain.Exceptions
{
    public class UnAuthorizeException : DomainException
    {
        public UnAuthorizeException() : base("Yetkinizden dolayı erişiminiz reddedildi.") { }
       
    }
}
