namespace SharedLibrary.Events
{
    // Customer Events
    public record CustomerCreatedEvent(
        Guid CustomerId,
        string FirstName,
        string LastName,
        string Email,
        DateTime CreatedAt
    );

    public record CustomerUpdatedEvent(
        Guid CustomerId,
        string FirstName,
        string LastName,
        string Email,
        DateTime UpdatedAt
    );

    public record CustomerDeletedEvent(
        Guid CustomerId,
        DateTime DeletedAt
    );

    // Product Events
    public record ProductCreatedEvent(
        Guid ProductId,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        DateTime CreatedAt
    );

    public record ProductUpdatedEvent(
        Guid ProductId,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        DateTime UpdatedAt
    );

    public record ProductPriceChangedEvent(
        Guid ProductId,
        decimal OldPrice,
        decimal NewPrice,
        DateTime UpdatedAt
    );

    public record ProductDeletedEvent(
        Guid ProductId,
        DateTime DeletedAt
    );

    public record ProductStockUpdatedEvent(
        Guid ProductId,
        int OldStock,
        int NewStock,
        DateTime UpdatedAt
    );

    // Order Events
    public record OrderCreatedEvent(
        Guid OrderId,
        string CustomerName,
        DateTime OrderDate,
        decimal TotalAmount,
        bool Status,
        DateTime CreatedAt
    );

    public record OrderUpdatedEvent(
        Guid OrderId,
        string CustomerName,
        DateTime OrderDate,
        decimal TotalAmount,
        bool Status,
        DateTime UpdatedAt
    );

    public record OrderStatusChangedEvent(
        Guid OrderId,
        bool OldStatus,
        bool NewStatus,
        DateTime UpdatedAt
    );

    public record OrderDeletedEvent(
        Guid OrderId,
        DateTime DeletedAt
    );
}