using MediatR;
using Order.Domain.Order;
using SharedLibrary.Common;

namespace Order.Application.Order.Queries.GetAllOrders
{
    public sealed record GetAllOrdersQuery : IRequest<BaseResponse<List<OrderDb>>>;
}