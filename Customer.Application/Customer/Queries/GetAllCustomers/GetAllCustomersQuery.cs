using Customer.Domain.Customer;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Queries.GetAllCustomers
{
    public sealed record GetAllCustomersQuery : IRequest<BaseResponse<List<CustomerDb>>>;
}