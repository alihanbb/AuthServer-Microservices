using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Customer.Application.Customer.Commands.CreateCustomer;
using Customer.Application.Customer.Commands.DeleteCustomer;
using Customer.Application.Customer.Commands.UpdateCustomer;
using Customer.Application.Customer.Queries.GetAllCustomers;
using Customer.Application.Customer.Queries.GetCustomerById;
using Customer.Application.Customer.Queries.GetCustomerByEmail;

namespace Customer.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteCustomerCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetAllCustomers(CancellationToken cancellationToken)
        {
            var query = await _mediator.Send(new GetAllCustomersQuery(), cancellationToken);
            return Ok(query);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetCustomerById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var queryResult = await _mediator.Send(new GetCustomerByIdQuery(id), cancellationToken);
            return Ok(queryResult);
        }

        [HttpGet("by-email/{email}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetCustomerByEmail([FromRoute] string email, CancellationToken cancellationToken)
        {
            var queryResult = await _mediator.Send(new GetCustomerByEmailQuery(email), cancellationToken);
            return Ok(queryResult);
        }
    }
}
