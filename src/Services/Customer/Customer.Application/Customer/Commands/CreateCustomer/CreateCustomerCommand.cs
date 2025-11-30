using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Commands.CreateCustomer
{
    public sealed record CreateCustomerCommand(
        string FirstName,
        string LastName,
        string Email) : IRequest<BaseResponse>;
}