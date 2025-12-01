using MediatR;

namespace Order.Application.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<CreateOrderResult>
{
    public Guid CustomerId { get; init; }
    public List<CreateOrderItemDto> Items { get; init; } = new();
}

public record CreateOrderItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public record CreateOrderResult
{
    public Guid OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
}

public enum OrderStatus
{
    Pending,
    Submitted,
    Completed,
    Failed,
    Cancelled
}
