using Customer.Domain.Customer;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Queries.GetCustomerById
{
    public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<BaseResponse<CustomerDb?>>;
}