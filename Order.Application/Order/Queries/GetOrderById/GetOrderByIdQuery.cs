using MediatR;
using Order.Domain.Order;
using SharedLibrary.Common;

namespace Order.Application.Order.Queries.GetOrderById
{
    public sealed record GetOrderByIdQuery(Guid Id) : IRequest<BaseResponse<OrderDb?>>;
}