using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Products;
using SharedBus.Messages.Events;

namespace Product.Application.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductDbContext _context;
    private readonly IPublishEndpoint _publisher;

    public CreateProductCommandHandler(IProductDbContext context, IPublishEndpoint publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish ProductCreatedEvent
        await _publisher.Publish(new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        }, cancellationToken);

        return new CreateProductResult
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}

// DbContext interface for dependency inversion
public interface IProductDbContext
{
    DbSet<ProductEntity> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
