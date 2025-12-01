using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Application.Commands.CreateOrder;
using Order.Domain.Order;
using SharedBus.Messages.Events;

namespace Order.Infrastructure.Consumers;

public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly IOrderDbContext _context;

    public OrderCompletedEventConsumer(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);

        if (order != null)
        {
            order.Status = OrderStatus.Completed;
            await _context.SaveChangesAsync();
        }
    }
}

public class OrderFailedEventConsumer : IConsumer<OrderFailedEvent>
{
    private readonly IOrderDbContext _context;

    public OrderFailedEventConsumer(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);

        if (order != null)
        {
            order.Status = OrderStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }
}
