using MediatR;
using Order.Application.Order.Services;
using Order.Domain.Order;
using SharedLibrary.Common;

namespace Order.Application.Order.Queries.GetOrderById
{
    public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, BaseResponse<OrderDb?>>
    {
        private readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseResponse<OrderDb?>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _orderService.GetOrderByIdAsync(request.Id, cancellationToken);
        }
    }
}