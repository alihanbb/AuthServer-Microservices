using Customer.Domain.Customer;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Queries.GetCustomerByEmail
{
    public sealed record GetCustomerByEmailQuery(string Email) : IRequest<BaseResponse<CustomerDb?>>;
}