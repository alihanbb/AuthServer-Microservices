namespace SharedBus.Messages.Events;

// Order Events
public record OrderSubmittedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public List<OrderItemData> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public DateTime SubmittedAt { get; init; }
}

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record OrderCompletedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid OrderId { get; init; }
    public DateTime CompletedAt { get; init; }
}

public record OrderFailedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime FailedAt { get; init; }
}

public record OrderCancelledEvent
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime CancelledAt { get; init; }
}

// Product Events
public record ProductCreatedEvent
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record ProductStockUpdatedEvent
{
    public Guid ProductId { get; init; }
    public int OldStock { get; init; }
    public int NewStock { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record ProductPriceChangedEvent
{
    public Guid ProductId { get; init; }
    public decimal OldPrice { get; init; }
    public decimal NewPrice { get; init; }
    public DateTime ChangedAt { get; init; }
}

public record ProductDeletedEvent
{
    public Guid ProductId { get; init; }
    public DateTime DeletedAt { get; init; }
}

public record StockReservedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public DateTime ReservedAt { get; init; }
}

public record StockReservationFailedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid ProductId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

// Customer Events
public record CustomerCreatedEvent
{
    public Guid CustomerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record CustomerValidatedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid CustomerId { get; init; }
    public bool IsValid { get; init; }
    public DateTime ValidatedAt { get; init; }
}

public record CustomerValidationFailedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid CustomerId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public record CustomerDeletedEvent
{
    public Guid CustomerId { get; init; }
    public DateTime DeletedAt { get; init; }
}

// Supporting Types
public record OrderItemData
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
