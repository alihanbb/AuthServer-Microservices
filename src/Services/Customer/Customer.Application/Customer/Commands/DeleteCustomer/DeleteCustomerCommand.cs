using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Commands.DeleteCustomer
{
    public sealed record DeleteCustomerCommand(Guid Id) : IRequest<BaseResponse>;
}