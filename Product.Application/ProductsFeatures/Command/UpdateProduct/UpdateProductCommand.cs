using MediatR;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.UpdateProduct
{
    public sealed record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, int StockQuantity) : IRequest<BaseResponse>;

    
    
}
