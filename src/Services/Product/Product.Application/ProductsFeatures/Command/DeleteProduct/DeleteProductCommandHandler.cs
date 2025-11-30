using MediatR;
using Product.Application.Services;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.DeleteProduct
{
    public sealed class DeleteProductCommandHandler(IProductService productService) : IRequestHandler<DeleteProductCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return await productService.DeleteProductAsync(request, cancellationToken);
        }
    }
}
