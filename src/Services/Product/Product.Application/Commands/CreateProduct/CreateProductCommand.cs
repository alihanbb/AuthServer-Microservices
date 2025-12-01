using MediatR;

namespace Product.Application.Commands.CreateProduct;

public record CreateProductCommand : IRequest<CreateProductResult>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}

public record CreateProductResult
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
