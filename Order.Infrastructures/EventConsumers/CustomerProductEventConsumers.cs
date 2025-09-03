using MassTransit;
using SharedLibrary.Events;
using Microsoft.Extensions.Logging;

namespace Order.Infrastructures.EventConsumers
{
    // Customer event consumers
    public class CustomerCreatedEventConsumer : IConsumer<CustomerCreatedEvent>
    {
        private readonly ILogger<CustomerCreatedEventConsumer> _logger;

        public CustomerCreatedEventConsumer(ILogger<CustomerCreatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
        {
            var customerEvent = context.Message;
            
            _logger.LogInformation("Order Service: New customer created - {CustomerId} {FirstName} {LastName}", 
                customerEvent.CustomerId, customerEvent.FirstName, customerEvent.LastName);

            // Here you could update local customer cache, validate orders for this customer, etc.
            await Task.CompletedTask;
        }
    }

    public class CustomerDeletedEventConsumer : IConsumer<CustomerDeletedEvent>
    {
        private readonly ILogger<CustomerDeletedEventConsumer> _logger;

        public CustomerDeletedEventConsumer(ILogger<CustomerDeletedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CustomerDeletedEvent> context)
        {
            var customerEvent = context.Message;
            
            _logger.LogInformation("Order Service: Customer deleted - {CustomerId}", customerEvent.CustomerId);

            // Here you could mark all orders for this customer as requiring review, 
            // or prevent new orders from this customer
            await Task.CompletedTask;
        }
    }

    // Product event consumers
    public class ProductPriceChangedEventConsumer : IConsumer<ProductPriceChangedEvent>
    {
        private readonly ILogger<ProductPriceChangedEventConsumer> _logger;

        public ProductPriceChangedEventConsumer(ILogger<ProductPriceChangedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductPriceChangedEvent> context)
        {
            var priceEvent = context.Message;
            
            _logger.LogInformation("Order Service: Product price changed - {ProductId} from {OldPrice} to {NewPrice}", 
                priceEvent.ProductId, priceEvent.OldPrice, priceEvent.NewPrice);

            // Here you could update pending order prices, notify customers of price changes, etc.
            await Task.CompletedTask;
        }
    }

    public class ProductStockUpdatedEventConsumer : IConsumer<ProductStockUpdatedEvent>
    {
        private readonly ILogger<ProductStockUpdatedEventConsumer> _logger;

        public ProductStockUpdatedEventConsumer(ILogger<ProductStockUpdatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductStockUpdatedEvent> context)
        {
            var stockEvent = context.Message;
            
            _logger.LogInformation("Order Service: Product stock updated - {ProductId} from {OldStock} to {NewStock}", 
                stockEvent.ProductId, stockEvent.OldStock, stockEvent.NewStock);

            // Here you could validate pending orders against available stock,
            // automatically fulfill orders that were waiting for stock, etc.
            await Task.CompletedTask;
        }
    }

    public class ProductDeletedEventConsumer : IConsumer<ProductDeletedEvent>
    {
        private readonly ILogger<ProductDeletedEventConsumer> _logger;

        public ProductDeletedEventConsumer(ILogger<ProductDeletedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
        {
            var productEvent = context.Message;
            
            _logger.LogInformation("Order Service: Product deleted - {ProductId}", productEvent.ProductId);

            // Here you could cancel pending orders for this product,
            // mark existing orders with deleted products for review, etc.
            await Task.CompletedTask;
        }
    }
}