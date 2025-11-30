using Order.Application.Order.Commands.CreateOrder;
using Order.Application.Order.Commands.DeleteOrder;
using Order.Application.Order.Commands.UpdateOrder;
using Order.Domain.Order;
using SharedLibrary.Common;

namespace Order.Application.Order.Services
{
    public interface IOrderService
    {
        Task<BaseResponse> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken);
        Task<BaseResponse> DeleteOrderAsync(DeleteOrderCommand command, CancellationToken cancellationToken);
        Task<BaseResponse> UpdateOrderAsync(UpdateOrderCommand command, CancellationToken cancellationToken);
        Task<BaseResponse<List<OrderDb>>> GetAllOrdersAsync(CancellationToken cancellationToken);
        Task<BaseResponse<OrderDb?>> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse> UpdateOrderStatusAsync(Guid orderId, bool newStatus, CancellationToken cancellationToken);
    }
}
