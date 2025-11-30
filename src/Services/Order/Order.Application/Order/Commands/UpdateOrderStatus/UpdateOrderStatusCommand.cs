using MediatR;
using SharedLibrary.Common;

namespace Order.Application.Order.Commands.UpdateOrderStatus
{
    public sealed record UpdateOrderStatusCommand(Guid OrderId, bool NewStatus) : IRequest<BaseResponse>;
}