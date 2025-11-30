using MediatR;
using SharedLibrary.Common;
using System.Collections.Generic;

namespace Order.Application.Order.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(
        string CustomerName,
        Guid CustomerId,  // Added for event publishing
        DateTime OrderDate,
        decimal TotalAmount,
        bool Status,
        List<OrderItemModel> Items = null) : IRequest<BaseResponse>;
        
    public class OrderItemModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}