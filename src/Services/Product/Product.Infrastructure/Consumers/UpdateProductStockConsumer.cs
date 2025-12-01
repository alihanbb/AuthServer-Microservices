using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Application.Commands.CreateProduct;
using SharedBus.Messages.Commands;
using SharedBus.Messages.Events;

namespace Product.Infrastructure.Consumers;

// Saga participant - handles stock reservation from Order saga
public class UpdateProductStockConsumer : IConsumer<UpdateProductStockCommand>
{
    private readonly IProductDbContext _context;
    private readonly IPublishEndpoint _publisher;

    public UpdateProductStockConsumer(IProductDbContext context, IPublishEndpoint publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task Consume(ConsumeContext<UpdateProductStockCommand> context)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == context.Message.ProductId);

        if (product == null)
        {
            await _publisher.Publish(new StockReservationFailedEvent
            {
                CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
                ProductId = context.Message.ProductId,
                Reason = "Product not found"
            });
            return;
        }

        if (context.Message.IsReservation)
        {
            // Reserve stock
            if (product.Stock >= context.Message.Quantity)
            {
                product.Stock -= context.Message.Quantity;
                await _context.SaveChangesAsync();

                await _publisher.Publish(new StockReservedEvent
                {
                    CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = context.Message.Quantity,
                    ReservedAt = DateTime.UtcNow
                });

                // Also publish stock updated event
                await _publisher.Publish(new ProductStockUpdatedEvent
                {
                    ProductId = product.Id,
                    OldStock = product.Stock + context.Message.Quantity,
                    NewStock = product.Stock,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                await _publisher.Publish(new StockReservationFailedEvent
                {
                    CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
                    ProductId = product.Id,
                    Reason = $"Insufficient stock. Available: {product.Stock}, Requested: {context.Message.Quantity}"
                });
            }
        }
        else
        {
            // Release stock (compensation)
            product.Stock += context.Message.Quantity;
            await _context.SaveChangesAsync();
        }
    }
}
