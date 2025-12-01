using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedBus.Messages.Events;

namespace Customer.Application.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerDbContext _context;
    private readonly IPublishEndpoint _publisher;

    public CreateCustomerCommandHandler(ICustomerDbContext context, IPublishEndpoint publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Domain.Customer.CustomerEntity
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish CustomerCreatedEvent
        await _publisher.Publish(new CustomerCreatedEvent
        {
            CustomerId = customer.Id,
            Name = $"{customer.FirstName} {customer.LastName}",
            Email = customer.Email,
            CreatedAt = customer.CreatedAt
        }, cancellationToken);

        return new CreateCustomerResult
        {
            CustomerId = customer.Id,
            Name = $"{customer.FirstName} {customer.LastName}",
            Email = customer.Email
        };
    }
}
