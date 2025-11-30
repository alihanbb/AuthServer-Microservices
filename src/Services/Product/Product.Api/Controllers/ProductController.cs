using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.ProductsFeatures.Command.CreateProduct;
using Product.Application.ProductsFeatures.Command.DeleteProduct;
using Product.Application.ProductsFeatures.Command.UpdatePrice;
using Product.Application.ProductsFeatures.Command.UpdateProduct;
using Product.Application.ProductsFeatures.Query.GetAllProduct;
using Product.Application.ProductsFeatures.Query.GetByIdProduct;

namespace Product.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);

        }
        [Authorize(Policy = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand(id);
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        [Authorize(Policy = "Staff")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
   
        public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
        {
            var query = await mediator.Send(new GetAllProductQuery(), cancellationToken);
            return Ok(query);
        }

        [HttpGet("{id}")]
    
        public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            
            var queryResult = await mediator.Send(new GetByIdProductQuery(id), cancellationToken);
            return Ok(queryResult);
        }
        [HttpPatch("{productId}/price")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProductPrice([FromRoute] Guid productId, [FromBody] decimal newPrice, CancellationToken cancellationToken)
        {
            var command = new UpdatePriceCommand(productId, newPrice);
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }

}
