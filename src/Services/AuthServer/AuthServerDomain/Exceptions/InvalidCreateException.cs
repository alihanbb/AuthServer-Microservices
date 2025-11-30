namespace AuthServer.Domain.Exceptions;

public class InvalidCreateException : DomainException
{
        public InvalidCreateException() : base("Geçersiz kullanıcı adı veya şifre") { }
}