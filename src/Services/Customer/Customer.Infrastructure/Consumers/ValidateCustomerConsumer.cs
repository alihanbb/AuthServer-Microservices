using MassTransit;
using Microsoft.EntityFrameworkCore;
using Customer.Application.Commands.CreateCustomer;
using SharedBus.Messages.Commands;
using SharedBus.Messages.Events;

namespace Customer.Infrastructure.Consumers;

// Saga participant - validates customer for Order saga
public class ValidateCustomerConsumer : IConsumer<ValidateCustomerCommand>
{
    private readonly ICustomerDbContext _context;
    private readonly IPublishEndpoint _publisher;

    public ValidateCustomerConsumer(ICustomerDbContext context, IPublishEndpoint publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task Consume(ConsumeContext<ValidateCustomerCommand> context)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == context.Message.CustomerId);

        if (customer == null)
        {
            await _publisher.Publish(new CustomerValidationFailedEvent
            {
                CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
                CustomerId = context.Message.CustomerId,
                Reason = "Customer not found"
            });
            return;
        }

        // Add any additional validation logic here (e.g., customer is active, has good standing, etc.)
        
        await _publisher.Publish(new CustomerValidatedEvent
        {
            CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
            CustomerId = customer.Id,
            IsValid = true,
            ValidatedAt = DateTime.UtcNow
        });
    }
}
