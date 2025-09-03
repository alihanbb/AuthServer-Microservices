using MediatR;
using Product.Application.Services;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.CreateProduct
{
    public sealed class CreateProductCommandHandler(IProductService productServices) : IRequestHandler<CreateProductCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            return await productServices.CreateProductAsync(request, cancellationToken);

        }
    }
}
