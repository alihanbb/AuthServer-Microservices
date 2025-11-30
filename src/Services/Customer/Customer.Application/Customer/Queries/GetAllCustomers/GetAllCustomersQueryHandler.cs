using Customer.Application.Customer.Services;
using Customer.Domain.Customer;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Queries.GetAllCustomers
{
    public sealed class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, BaseResponse<List<CustomerDb>>>
    {
        private readonly ICustomerService _customerService;

        public GetAllCustomersQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<BaseResponse<List<CustomerDb>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            return await _customerService.GetAllCustomersAsync(cancellationToken);
        }
    }
}