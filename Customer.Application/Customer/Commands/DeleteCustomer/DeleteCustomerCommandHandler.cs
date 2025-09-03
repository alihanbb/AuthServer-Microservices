using Customer.Application.Customer.Services;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Commands.DeleteCustomer
{
    public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, BaseResponse>
    {
        private readonly ICustomerService _customerService;

        public DeleteCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<BaseResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            return await _customerService.DeleteCustomerAsync(request, cancellationToken);
        }
    }
}