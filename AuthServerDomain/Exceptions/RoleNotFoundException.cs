namespace AuthServer.Domain.Exceptions;

public class RoleNotFoundException : DomainException
{
    public RoleNotFoundException(string roleName) : base($"{roleName} adına sahip bir rol bulunamadı") { }
    
    
}
