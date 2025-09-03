using MassTransit;
using SharedLibrary.Events;
using Microsoft.Extensions.Logging;

namespace Product.Infrastructure.EventConsumers
{
    // Order event consumers
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventConsumer> _logger;

        public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var orderEvent = context.Message;
            
            _logger.LogInformation("Product Service: New order created - Order ID: {OrderId}, Amount: {TotalAmount}", 
                orderEvent.OrderId, orderEvent.TotalAmount);

            // Here you could:
            // - Reserve stock for ordered products
            // - Update product demand analytics
            // - Trigger automatic reordering if stock is low
            // - Update product popularity metrics
            await Task.CompletedTask;
        }
    }

    public class OrderStatusChangedEventConsumer : IConsumer<OrderStatusChangedEvent>
    {
        private readonly ILogger<OrderStatusChangedEventConsumer> _logger;

        public OrderStatusChangedEventConsumer(ILogger<OrderStatusChangedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
        {
            var statusEvent = context.Message;
            
            _logger.LogInformation("Product Service: Order status changed - Order ID: {OrderId}, Status: {OldStatus} -> {NewStatus}", 
                statusEvent.OrderId, statusEvent.OldStatus, statusEvent.NewStatus);

            // Here you could:
            // - If order completed: reduce actual stock
            // - If order cancelled: release reserved stock
            // - Update product availability status
            // - Trigger stock replenishment if needed
            await Task.CompletedTask;
        }
    }

    public class OrderDeletedEventConsumer : IConsumer<OrderDeletedEvent>
    {
        private readonly ILogger<OrderDeletedEventConsumer> _logger;

        public OrderDeletedEventConsumer(ILogger<OrderDeletedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderDeletedEvent> context)
        {
            var orderEvent = context.Message;
            
            _logger.LogInformation("Product Service: Order deleted - Order ID: {OrderId}", orderEvent.OrderId);

            // Here you could:
            // - Release any reserved stock
            // - Update product demand analytics
            // - Adjust pricing algorithms based on cancelled orders
            // - Update product recommendation systems
            await Task.CompletedTask;
        }
    }
}