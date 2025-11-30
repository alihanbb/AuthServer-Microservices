using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Order.Commands.CreateOrder;
using Order.Application.Order.Commands.DeleteOrder;
using Order.Application.Order.Commands.UpdateOrder;
using Order.Application.Order.Commands.UpdateOrderStatus;
using Order.Application.Order.Queries.GetAllOrders;
using Order.Application.Order.Queries.GetOrderById;

namespace Order.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteOrderCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
        {
            var query = await _mediator.Send(new GetAllOrdersQuery(), cancellationToken);
            return Ok(query);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var queryResult = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
            return Ok(queryResult);
        }

        [HttpPatch("{orderId}/status")]
        [Authorize(Policy = "StaffOnly")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromBody] bool newStatus, CancellationToken cancellationToken)
        {
            var command = new UpdateOrderStatusCommand(orderId, newStatus);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
