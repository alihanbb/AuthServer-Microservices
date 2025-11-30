using MediatR;
using Order.Application.Order.Services;
using Order.Domain.Order;
using SharedLibrary.Common;

namespace Order.Application.Order.Queries.GetAllOrders
{
    public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, BaseResponse<List<OrderDb>>>
    {
        private readonly IOrderService _orderService;

        public GetAllOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseResponse<List<OrderDb>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _orderService.GetAllOrdersAsync(cancellationToken);
        }
    }
}