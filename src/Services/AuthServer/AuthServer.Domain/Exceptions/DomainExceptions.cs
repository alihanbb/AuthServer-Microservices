namespace AuthServer.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string userName) 
        : base($"{userName} adına sahip bir kullanıcı bulunamadı") { }
}

public class RoleNotFoundException : DomainException
{
    public RoleNotFoundException(string roleName) 
        : base($"{roleName} adına sahip bir rol bulunamadı") { }
}

public class DomainInvalidOperationException : DomainException
{
    public DomainInvalidOperationException(string message) : base(message) { }
}
