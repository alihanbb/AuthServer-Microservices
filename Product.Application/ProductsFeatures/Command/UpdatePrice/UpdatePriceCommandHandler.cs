using MediatR;
using Product.Application.Services;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.UpdatePrice
{
    public class UpdatePriceCommandHandler(IProductService productService) : IRequestHandler<UpdatePriceCommand, BaseResponse>
    {
      
        public async Task<BaseResponse> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
        {
            return await productService.UpdateProductPriceAsync(request.ProductId, request.NewPrice, cancellationToken);
        }
    }
}
