using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedBus.Constants;
using SharedBus.Sagas;

namespace SharedBus.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddSharedBusWithAzureServiceBus(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        services.AddMassTransit(x =>
        {
            // Configure consumers if provided
            configureConsumers?.Invoke(x);

            // Configure Azure Service Bus
            x.UsingAzureServiceBus((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("AzureServiceBus");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Azure Service Bus connection string is not configured.");
                }

                cfg.Host(connectionString);

                // Configure message topology
                cfg.Message<Messages.Events.OrderSubmittedEvent>(e => e.SetEntityName(TopicNames.OrderEventsTopic));
                cfg.Message<Messages.Events.OrderCreatedEvent>(e => e.SetEntityName(TopicNames.OrderEventsTopic));
                cfg.Message<Messages.Events.OrderCompletedEvent>(e => e.SetEntityName(TopicNames.OrderEventsTopic));
                cfg.Message<Messages.Events.OrderFailedEvent>(e => e.SetEntityName(TopicNames.OrderEventsTopic));
                cfg.Message<Messages.Events.OrderCancelledEvent>(e => e.SetEntityName(TopicNames.OrderEventsTopic));
                
                cfg.Message<Messages.Events.ProductCreatedEvent>(e => e.SetEntityName(TopicNames.ProductEventsTopic));
                cfg.Message<Messages.Events.ProductStockUpdatedEvent>(e => e.SetEntityName(TopicNames.ProductEventsTopic));
                cfg.Message<Messages.Events.ProductPriceChangedEvent>(e => e.SetEntityName(TopicNames.ProductEventsTopic));
                cfg.Message<Messages.Events.ProductDeletedEvent>(e => e.SetEntityName(TopicNames.ProductEventsTopic));
                cfg.Message<Messages.Events.StockReservedEvent>(e => e.SetEntityName(TopicNames.ProductEventsTopic));
                cfg.Message<Messages.Events.StockReservationFailedEvent>(e => e.SetEntityName(TopicNames.ProductEventsTopic));
                
                cfg.Message<Messages.Events.CustomerCreatedEvent>(e => e.SetEntityName(TopicNames.CustomerEventsTopic));
                cfg.Message<Messages.Events.CustomerValidatedEvent>(e => e.SetEntityName(TopicNames.CustomerEventsTopic));
                cfg.Message<Messages.Events.CustomerValidationFailedEvent>(e => e.SetEntityName(TopicNames.CustomerEventsTopic));
                cfg.Message<Messages.Events.CustomerDeletedEvent>(e => e.SetEntityName(TopicNames.CustomerEventsTopic));

                // Configure endpoints
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection AddOrderSaga(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>()
                .InMemoryRepository(); // Use EntityFrameworkRepository for production

            x.UsingAzureServiceBus((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("AzureServiceBus");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Azure Service Bus connection string is not configured.");
                }

                cfg.Host(connectionString);

                cfg.ReceiveEndpoint(QueueNames.SagaOrderStateQueue, e =>
                {
                    e.ConfigureSaga<OrderSagaState>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
