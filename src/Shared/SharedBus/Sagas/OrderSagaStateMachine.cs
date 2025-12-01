using MassTransit;
using SharedBus.Messages.Commands;
using SharedBus.Messages.Events;

namespace SharedBus.Sagas;

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public OrderSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => CustomerValidated, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => CustomerValidationFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => StockReserved, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => StockReservationFailed, x => x.CorrelateById(m => m.Message.CorrelationId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                    context.Saga.SubmittedAt = context.Message.SubmittedAt;
                    context.Saga.ProductsJson = System.Text.Json.JsonSerializer.Serialize(context.Message.Items);
                })
                .Publish(context => new ValidateCustomerCommand
                {
                    CustomerId = context.Saga.CustomerId,
                    OrderId = context.Saga.OrderId
                })
                .TransitionTo(ValidatingCustomer)
        );

        During(ValidatingCustomer,
            When(CustomerValidated)
                .Then(context =>
                {
                    context.Saga.CustomerValidated = true;
                })
                .If(context => context.Message.IsValid,
                    binder => binder
                        .PublishAsync(context =>
                        {
                            var items = System.Text.Json.JsonSerializer.Deserialize<List<OrderItemData>>(context.Saga.ProductsJson ?? "[]");
                            return Task.FromResult(items?.Select(item => new UpdateProductStockCommand
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                IsReservation = true
                            }) ?? Enumerable.Empty<UpdateProductStockCommand>());
                        })
                        .TransitionTo(ReservingStock))
                .If(context => !context.Message.IsValid,
                    binder => binder
                        .Publish(context => new OrderFailedEvent
                        {
                            CorrelationId = context.Saga.CorrelationId,
                            OrderId = context.Saga.OrderId,
                            Reason = "Customer validation failed",
                            FailedAt = DateTime.UtcNow
                        })
                        .TransitionTo(Failed)),
                        
            When(CustomerValidationFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.FailedAt = DateTime.UtcNow;
                })
                .Publish(context => new OrderFailedEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    OrderId = context.Saga.OrderId,
                    Reason = context.Saga.FailureReason ?? "Customer validation failed",
                    FailedAt = context.Saga.FailedAt ?? DateTime.UtcNow
                })
                .TransitionTo(Failed)
        );

        During(ReservingStock,
            When(StockReserved)
                .Then(context =>
                {
                    context.Saga.StockReserved = true;
                })
                .Publish(context => new OrderCompletedEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    OrderId = context.Saga.OrderId,
                    CompletedAt = DateTime.UtcNow
                })
                .TransitionTo(Completed)
                .Finalize(),
                
            When(StockReservationFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.FailedAt = DateTime.UtcNow;
                })
                .Publish(context => new OrderFailedEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    OrderId = context.Saga.OrderId,
                    Reason = context.Saga.FailureReason ?? "Stock reservation failed",
                    FailedAt = context.Saga.FailedAt ?? DateTime.UtcNow
                })
                .TransitionTo(Failed)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    // States
    public State ValidatingCustomer { get; private set; } = null!;
    public State ReservingStock { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    // Events
    public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; } = null!;
    public Event<CustomerValidatedEvent> CustomerValidated { get; private set; } = null!;
    public Event<CustomerValidationFailedEvent> CustomerValidationFailed { get; private set; } = null!;
    public Event<StockReservedEvent> StockReserved { get; private set; } = null!;
    public Event<StockReservationFailedEvent> StockReservationFailed { get; private set; } = null!;
}
