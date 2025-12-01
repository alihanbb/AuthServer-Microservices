namespace SharedBus.Constants;

public static class QueueNames
{
    // Command Queues
    public const string OrderCommandQueue = "order-command-queue";
    public const string ProductCommandQueue = "product-command-queue";
    public const string CustomerCommandQueue = "customer-command-queue";
    
    // Saga State Queue
    public const string SagaOrderStateQueue = "saga-order-state-queue";
}

public static class TopicNames
{
    // Event Topics
    public const string OrderEventsTopic = "order-events-topic";
    public const string ProductEventsTopic = "product-events-topic";
    public const string CustomerEventsTopic = "customer-events-topic";
}

public static class SubscriptionNames
{
    // Order Service Subscriptions
    public const string OrderServiceProductEvents = "order-service-product-events";
    public const string OrderServiceCustomerEvents = "order-service-customer-events";
    
    // Product Service Subscriptions
    public const string ProductServiceOrderEvents = "product-service-order-events";
    
    // Customer Service Subscriptions
    public const string CustomerServiceOrderEvents = "customer-service-order-events";
}
