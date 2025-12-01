using MassTransit;

namespace SharedBus.Sagas;

public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    
    // Order Information
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Tracking
    public bool CustomerValidated { get; set; }
    public bool StockReserved { get; set; }
    public bool PaymentProcessed { get; set; }
    
    // Timestamps
    public DateTime SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    
    // Failure Information
    public string? FailureReason { get; set; }
    
    // Products to reserve
    public string? ProductsJson { get; set; } // JSON serialized list of products
}
