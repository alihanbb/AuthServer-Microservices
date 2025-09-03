using MediatR;
using SharedLibrary.Common;

namespace Order.Application.Order.Commands.UpdateOrder
{
    public sealed record UpdateOrderCommand(
        Guid Id, 
        string CustomerName, 
        DateTime OrderDate, 
        decimal TotalAmount, 
        bool Status) : IRequest<BaseResponse>;
}