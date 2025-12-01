using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Order;
using SharedBus.Messages.Events;

namespace Order.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderDbContext _context;
    private readonly IPublishEndpoint _publisher;

    public CreateOrderCommandHandler(IOrderDbContext context, IPublishEndpoint publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Create order entity
        var order = new OrderEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            OrderItems = request.Items.Select(item => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish OrderSubmittedEvent to start the saga
        var orderSubmittedEvent = new OrderSubmittedEvent
        {
            CorrelationId = Guid.NewGuid(),
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            Items = order.OrderItems.Select(item => new OrderItemData
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList(),
            TotalAmount = order.TotalAmount,
            SubmittedAt = DateTime.UtcNow
        };

        await _publisher.Publish(orderSubmittedEvent, cancellationToken);

        // Update order status to Submitted
        order.Status = OrderStatus.Submitted;
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult
        {
            OrderId = order.Id,
            Status = order.Status,
            TotalAmount = order.TotalAmount
        };
    }
}

// DbContext interface for dependency inversion
public interface IOrderDbContext
{
    DbSet<OrderEntity> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
