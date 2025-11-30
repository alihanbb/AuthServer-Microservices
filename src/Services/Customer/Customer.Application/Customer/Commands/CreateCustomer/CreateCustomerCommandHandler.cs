using Customer.Application.Customer.Services;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Commands.CreateCustomer
{
    public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, BaseResponse>
    {
        private readonly ICustomerService _customerService;

        public CreateCustomerCommandHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<BaseResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            return await _customerService.CreateCustomerAsync(request, cancellationToken);
        }
    }
}