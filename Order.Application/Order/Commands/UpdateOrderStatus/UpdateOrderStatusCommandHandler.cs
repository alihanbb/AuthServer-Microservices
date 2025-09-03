using MediatR;
using Order.Application.Order.Services;
using SharedLibrary.Common;

namespace Order.Application.Order.Commands.UpdateOrderStatus
{
    public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, BaseResponse>
    {
        private readonly IOrderService _orderService;

        public UpdateOrderStatusCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            return await _orderService.UpdateOrderStatusAsync(request.OrderId, request.NewStatus, cancellationToken);
        }
    }
}