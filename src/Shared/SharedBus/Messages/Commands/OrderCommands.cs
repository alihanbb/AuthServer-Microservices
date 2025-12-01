namespace SharedBus.Messages.Commands;

public record CreateOrderCommand
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public DateTime OrderDate { get; init; }
}

public record OrderItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public record CancelOrderCommand
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public record UpdateProductStockCommand
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public bool IsReservation { get; init; }
}

public record ValidateCustomerCommand
{
    public Guid CustomerId { get; init; }
    public Guid OrderId { get; init; }
}
