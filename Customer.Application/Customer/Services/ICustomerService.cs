using Customer.Application.Customer.Commands.CreateCustomer;
using Customer.Application.Customer.Commands.DeleteCustomer;
using Customer.Application.Customer.Commands.UpdateCustomer;
using Customer.Domain.Customer;
using SharedLibrary.Common;

namespace Customer.Application.Customer.Services
{
    public interface ICustomerService
    {
        Task<BaseResponse> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken);
        Task<BaseResponse> DeleteCustomerAsync(DeleteCustomerCommand command, CancellationToken cancellationToken);
        Task<BaseResponse> UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken);
        Task<BaseResponse<List<CustomerDb>>> GetAllCustomersAsync(CancellationToken cancellationToken);
        Task<BaseResponse<CustomerDb?>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse<CustomerDb?>> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken);
    }
}