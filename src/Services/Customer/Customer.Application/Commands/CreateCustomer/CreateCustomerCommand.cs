using MediatR;

namespace Customer.Application.Commands.CreateCustomer;

public record CreateCustomerCommand : IRequest<CreateCustomerResult>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}

public record CreateCustomerResult
{
    public Guid CustomerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

// DbContext interface
public interface ICustomerDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<Customer.Domain.Customer.CustomerEntity> Customers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
