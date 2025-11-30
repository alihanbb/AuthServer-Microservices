using MediatR;
using SharedLibrary.Common;

namespace Order.Application.Order.Commands.DeleteOrder
{
    public sealed record DeleteOrderCommand(Guid Id) : IRequest<BaseResponse>;
}