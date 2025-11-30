using MediatR;
using Order.Application.Order.Services;
using SharedLibrary.Common;

namespace Order.Application.Order.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, BaseResponse>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            return await _orderService.CreateOrderAsync(request, cancellationToken);
        }
    }
}