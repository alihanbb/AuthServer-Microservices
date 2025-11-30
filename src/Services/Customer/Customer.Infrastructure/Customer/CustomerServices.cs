using AutoMapper;
using Customer.Application.Customer.Commands.CreateCustomer;
using Customer.Application.Customer.Commands.DeleteCustomer;
using Customer.Application.Customer.Commands.UpdateCustomer;
using Customer.Application.Customer.Services;
using Customer.Domain.Customer;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Common;
using SharedLibrary.Events;
using SharedLibrary.Messaging;

namespace Customer.Infrastructure.Customer
{
    public class CustomerServices : ICustomerService
    {
        private readonly CustomerDbContext _customerDbContext;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public CustomerServices(
            CustomerDbContext customerDbContext, 
            IMapper mapper,
            IEventPublisher eventPublisher)
        {
            _customerDbContext = customerDbContext;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<BaseResponse> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var createCustomer = _mapper.Map<CustomerDb>(command);
                createCustomer.CreatedAt = DateTime.UtcNow;
                createCustomer.UpdatedAt = DateTime.UtcNow;
                createCustomer.IsDeleted = false;

                await _customerDbContext.Customers.AddAsync(createCustomer, cancellationToken);
                await _customerDbContext.SaveChangesAsync(cancellationToken);
                
                // Publish CustomerCreated event
                var customerCreatedEvent = new CustomerCreatedEvent(
                    createCustomer.Id,
                    createCustomer.FirstName,
                    createCustomer.LastName,
                    createCustomer.Email,
                    createCustomer.CreatedAt
                );
                
                await _eventPublisher.PublishAsync(customerCreatedEvent, cancellationToken);
                
                return new CreatedResponse("Müşteri başarıyla oluşturuldu", 201, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Müşteri oluşturulurken hata oluştu: {ex.Message}", 500, false);
            }
        }

        public async Task<BaseResponse> DeleteCustomerAsync(DeleteCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var deleteCustomer = await _customerDbContext.Customers
                    .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

                if (deleteCustomer is null)
                    return new ErrorResponse($"Müşteri bulunamadı. ID: {command.Id} ", 404, false);

                if (deleteCustomer.IsDeleted)
                    return new ErrorResponse($"Müşteri zaten silinmiş durumda. ID: {command.Id}", 400, false);

                deleteCustomer.IsDeleted = true;
                deleteCustomer.DeletedAt = DateTime.UtcNow;
                deleteCustomer.UpdatedAt = DateTime.UtcNow;

                await _customerDbContext.SaveChangesAsync(cancellationToken);

                // Publish CustomerDeleted event
                var customerDeletedEvent = new CustomerDeletedEvent(
                    deleteCustomer.Id,
                    deleteCustomer.DeletedAt
                );
                
                await _eventPublisher.PublishAsync(customerDeletedEvent, cancellationToken);

                return new SuccessResponse($"Müşteri başarıyla silindi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Müşteri silinirken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse<List<CustomerDb>>> GetAllCustomersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var listCustomers = await _customerDbContext.Customers
                    .Where(c => !c.IsDeleted)
                    .ToListAsync(cancellationToken);
                return new SuccessResponse<List<CustomerDb>>(listCustomers, "Müşteriler başarıyla listelendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse<List<CustomerDb>>(null, $"Müşteriler listelenirken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse<CustomerDb?>> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerDbContext.Customers
                    .FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted, cancellationToken);

                if (customer is null)
                    return new ErrorResponse<CustomerDb?>(null, $"Müşteri bulunamadı. Email: {email}", 404, false);

                var customerDto = _mapper.Map<CustomerDb>(customer);
                return new SuccessResponse<CustomerDb?>(customerDto, "Müşteri başarıyla bulundu.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse<CustomerDb?>(null, $"Müşteri bulunurken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse<CustomerDb?>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerDbContext.Customers
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);

                if (customer is null)
                    return new ErrorResponse<CustomerDb?>(null, $"Müşteri bulunamadı. ID: {id}", 404, false);

                var customerDto = _mapper.Map<CustomerDb>(customer);
                return new SuccessResponse<CustomerDb?>(customerDto, "Müşteri başarıyla bulundu.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse<CustomerDb?>(null, $"Müşteri bulunurken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse> UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var updateCustomer = await _customerDbContext.Customers.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                if (updateCustomer is null)
                    return new ErrorResponse($"Müşteri bulunamadı. ID: {command.Id}", 404, false);

                var customerDto = _mapper.Map(command, updateCustomer);
                customerDto.UpdatedAt = DateTime.UtcNow;
                await _customerDbContext.SaveChangesAsync(cancellationToken);

                // Publish CustomerUpdated event
                var customerUpdatedEvent = new CustomerUpdatedEvent(
                    customerDto.Id,
                    customerDto.FirstName,
                    customerDto.LastName,
                    customerDto.Email,
                    customerDto.UpdatedAt
                );
                
                await _eventPublisher.PublishAsync(customerUpdatedEvent, cancellationToken);

                return new SuccessResponse("Müşteri başarıyla güncellendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Müşteri güncellenirken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }
    }
}
