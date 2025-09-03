using AutoMapper;
using Order.Application.Order.Commands.CreateOrder;
using Order.Application.Order.Commands.DeleteOrder;
using Order.Application.Order.Commands.UpdateOrder;
using Order.Application.Order.Commands.UpdateOrderStatus;
using Order.Domain.Order;

namespace Order.Infrastructures.Order
{
    public sealed class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<CreateOrderCommand, OrderDb>().ReverseMap();
            CreateMap<UpdateOrderCommand, OrderDb>().ReverseMap();
            CreateMap<DeleteOrderCommand, OrderDb>().ReverseMap();
            CreateMap<UpdateOrderStatusCommand, OrderDb>().ReverseMap();
        }
    }
}
