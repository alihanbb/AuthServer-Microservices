using AutoMapper;
using Order.Application.Order.Commands.CreateOrder;
using Order.Application.Order.Commands.DeleteOrder;
using Order.Application.Order.Commands.UpdateOrder;
using Order.Application.Order.Services;
using Order.Domain.Order;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Common;
using SharedLibrary.Events;
using SharedLibrary.Messaging;

namespace Order.Infrastructures.Order
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public OrderService(OrderDbContext orderDbContext, IMapper mapper, IEventPublisher eventPublisher)
        {
            _orderDbContext = orderDbContext;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<BaseResponse> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var createOrder = _mapper.Map<OrderDb>(command);
                createOrder.CreatedAt = DateTime.UtcNow;
                createOrder.UpdatedAt = DateTime.UtcNow;
                createOrder.IsDeleted = false;

                await _orderDbContext.Orders.AddAsync(createOrder, cancellationToken);
                await _orderDbContext.SaveChangesAsync(cancellationToken);

                // Publish OrderCreated event
                var orderCreatedEvent = new OrderCreatedEvent(
                    createOrder.Id,
                    createOrder.CustomerName,
                    createOrder.OrderDate,
                    createOrder.TotalAmount,
                    createOrder.Status,
                    createOrder.CreatedAt
                );
                
                await _eventPublisher.PublishAsync(orderCreatedEvent, cancellationToken);

                return new CreatedResponse("Sipariş başarıyla oluşturuldu", 201, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Sipariş oluşturulurken hata oluştu: {ex.Message}", 500, false);
            }
        }

        public async Task<BaseResponse> DeleteOrderAsync(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var deleteOrder = await _orderDbContext.Orders
                    .FirstOrDefaultAsync(o => o.Id == command.Id, cancellationToken);

                if (deleteOrder is null)
                    return new ErrorResponse($"Sipariş bulunamadı. ID: {command.Id} ", 404, false);

                if (deleteOrder.IsDeleted)
                    return new ErrorResponse($"Sipariş zaten silinmiş durumda. ID: {command.Id}", 400, false);

                deleteOrder.IsDeleted = true;
                deleteOrder.DeletedAt = DateTime.UtcNow;
                deleteOrder.UpdatedAt = DateTime.UtcNow;

                await _orderDbContext.SaveChangesAsync(cancellationToken);

                // Publish OrderDeleted event
                var orderDeletedEvent = new OrderDeletedEvent(
                    deleteOrder.Id,
                    deleteOrder.DeletedAt.Value
                );
                
                await _eventPublisher.PublishAsync(orderDeletedEvent, cancellationToken);

                return new SuccessResponse($"Sipariş başarıyla silindi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Sipariş silinirken hata oluştu: {ex.Message}", 500, false);
            }
        }

        public async Task<BaseResponse<List<OrderDb>>> GetAllOrdersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var listOrders = await _orderDbContext.Orders
                    .Where(o => !o.IsDeleted)
                    .ToListAsync(cancellationToken);
                return new SuccessResponse<List<OrderDb>>(listOrders, "Siparişler başarıyla listelendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse<List<OrderDb>>(null, $"Siparişler listelenirken hata oluştu: {ex.Message}", 500, false);
            }
        }

        public async Task<BaseResponse<OrderDb?>> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderDbContext.Orders
                    .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, cancellationToken);

                if (order is null)
                    return new ErrorResponse<OrderDb?>(null, $"Sipariş bulunamadı. ID: {id}", 404, false);

                return new SuccessResponse<OrderDb?>(order, "Sipariş başarıyla bulundu.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse<OrderDb?>(null, $"Sipariş bulunurken hata oluştu: {ex.Message}", 500, false);
            }
        }

        public async Task<BaseResponse> UpdateOrderAsync(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var updateOrder = await _orderDbContext.Orders.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                if (updateOrder is null)
                    return new ErrorResponse($"Sipariş bulunamadı. ID: {command.Id}", 404, false);

                // Store previous values for event
                var previousTotalAmount = updateOrder.TotalAmount;

                var orderDb = _mapper.Map(command, updateOrder);
                orderDb.UpdatedAt = DateTime.UtcNow;
                await _orderDbContext.SaveChangesAsync(cancellationToken);

                // Publish OrderUpdated event
                var orderUpdatedEvent = new OrderUpdatedEvent(
                    orderDb.Id,
                    orderDb.CustomerName,
                    orderDb.OrderDate,
                    orderDb.TotalAmount,
                    orderDb.Status,
                    orderDb.UpdatedAt
                );
                
                await _eventPublisher.PublishAsync(orderUpdatedEvent, cancellationToken);

                return new SuccessResponse("Sipariş başarıyla güncellendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Sipariş güncellenirken hata oluştu: {ex.Message}", 500, false);
            }
        }

        public async Task<BaseResponse> UpdateOrderStatusAsync(Guid orderId, bool newStatus, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderDbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
                if (order is null)
                    return new ErrorResponse($"Sipariş bulunamadı. ID: {orderId}", 404, false);

                // Store previous status
                var previousStatus = order.Status;

                // Only update and publish event if status actually changed
                if (previousStatus != newStatus)
                {
                    order.Status = newStatus;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderDbContext.SaveChangesAsync(cancellationToken);

                    // Publish OrderStatusChanged event
                    var orderStatusChangedEvent = new OrderStatusChangedEvent(
                        order.Id,
                        previousStatus,
                        newStatus,
                        order.UpdatedAt
                    );
                    
                    await _eventPublisher.PublishAsync(orderStatusChangedEvent, cancellationToken);
                }

                return new SuccessResponse("Sipariş durumu başarıyla güncellendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Sipariş durumu güncellenirken hata oluştu: {ex.Message}", 500, false);
            }
        }
    }
}