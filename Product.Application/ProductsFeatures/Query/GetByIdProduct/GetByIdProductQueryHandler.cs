using MediatR;
using Product.Application.Services;
using Product.Domain.Entities;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Query.GetByIdProduct
{
    public sealed class GetByIdProductQueryHandler(IProductService productService) : IRequestHandler<GetByIdProductQuery, BaseResponse<ProductBase>>
    {
        public async Task<BaseResponse<ProductBase>> Handle(GetByIdProductQuery request, CancellationToken cancellationToken)
        {
           return await productService.GetProductByIdAsync(request.Id, cancellationToken);
        }
    }
    
}
