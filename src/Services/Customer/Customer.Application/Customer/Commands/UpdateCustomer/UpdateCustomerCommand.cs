using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Commands.UpdateCustomer
{
    public sealed record UpdateCustomerCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email) : IRequest<BaseResponse>;
}