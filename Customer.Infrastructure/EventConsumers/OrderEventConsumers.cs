using MassTransit;
using SharedLibrary.Events;
using Microsoft.Extensions.Logging;

namespace Customer.Infrastructure.EventConsumers
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
            
            _logger.LogInformation("Customer Service: New order created for customer - {CustomerName}, Order ID: {OrderId}, Amount: {TotalAmount}", 
                orderEvent.CustomerName, orderEvent.OrderId, orderEvent.TotalAmount);

            // Here you could:
            // - Update customer statistics (total orders, total spent)
            // - Send welcome email for first order
            // - Update customer loyalty points
            // - Track customer purchase behavior
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
            
            _logger.LogInformation("Customer Service: Order status changed - Order ID: {OrderId}, Status: {OldStatus} -> {NewStatus}", 
                statusEvent.OrderId, statusEvent.OldStatus, statusEvent.NewStatus);

            // Here you could:
            // - Send notification to customer about status change
            // - Update customer dashboard
            // - Trigger email notifications
            // - Update customer satisfaction metrics
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
            
            _logger.LogInformation("Customer Service: Order deleted - Order ID: {OrderId}", orderEvent.OrderId);

            // Here you could:
            // - Update customer statistics
            // - Send cancellation confirmation
            // - Process refunds if needed
            // - Update customer purchase history
            await Task.CompletedTask;
        }
    }
}