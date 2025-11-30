using Customer.Application.Customer.Services;
using Customer.Domain.Customer;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Queries.GetCustomerByEmail
{
    public sealed class GetCustomerByEmailQueryHandler : IRequestHandler<GetCustomerByEmailQuery, BaseResponse<CustomerDb?>>
    {
        private readonly ICustomerService _customerService;

        public GetCustomerByEmailQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<BaseResponse<CustomerDb?>> Handle(GetCustomerByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _customerService.GetCustomerByEmailAsync(request.Email, cancellationToken);
        }
    }
}