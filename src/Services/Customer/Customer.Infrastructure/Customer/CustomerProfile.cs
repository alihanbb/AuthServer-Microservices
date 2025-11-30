using AutoMapper;
using Customer.Application.Customer.Commands.CreateCustomer;
using Customer.Application.Customer.Commands.UpdateCustomer;
using Customer.Application.Customer.Queries.GetCustomerById;
using Customer.Domain.Customer;

namespace Customer.Infrastructure.Customer
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CreateCustomerCommand, CustomerDb>().ReverseMap();
            CreateMap<UpdateCustomerCommand, CustomerDb>().ReverseMap();
            CreateMap<GetCustomerByIdQuery, CustomerDb>().ReverseMap();
        }
    }
}
