
namespace AuthServer.Domain.Exceptions;

public class UserNotFoundException : DomainException
{

    public UserNotFoundException(string userName) : base($"{userName} adına sahip bir kullanıcı bulunamadı") { }


}
