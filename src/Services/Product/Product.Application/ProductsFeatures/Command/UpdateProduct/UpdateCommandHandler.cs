using MediatR;
using Product.Application.Services;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.UpdateProduct
{
    public class UpdateCommandHandler(IProductService productService) : IRequestHandler<UpdateProductCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            return await productService.UpdateProductAsync(request, cancellationToken);
        }
    }
}
