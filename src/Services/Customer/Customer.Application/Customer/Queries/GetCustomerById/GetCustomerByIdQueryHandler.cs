using Customer.Application.Customer.Services;
using Customer.Domain.Customer;
using MediatR;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Queries.GetCustomerById
{
    public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, BaseResponse<CustomerDb?>>
    {
        private readonly ICustomerService _customerService;

        public GetCustomerByIdQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<BaseResponse<CustomerDb?>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _customerService.GetCustomerByIdAsync(request.Id, cancellationToken);
        }
    }
}