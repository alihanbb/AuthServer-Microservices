using Customer.Application.Customer.Services;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Commands.UpdateCustomer
{
    public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, BaseResponse>
    {
        private readonly ICustomerService _customerService;

        public UpdateCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<BaseResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            return await _customerService.UpdateCustomerAsync(request, cancellationToken);
        }
    }
}