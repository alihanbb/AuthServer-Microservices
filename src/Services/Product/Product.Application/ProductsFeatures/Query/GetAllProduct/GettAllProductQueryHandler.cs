using MediatR;
using Product.Application.Services;
using Product.Domain.Entities;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Query.GetAllProduct
{
    public class GettAllProductQueryHandler(IProductService productService) : IRequestHandler<GetAllProductQuery, BaseResponse<List<ProductBase>>>
    {
        public async Task<BaseResponse<List<ProductBase>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            return await productService.GetAllProductsAsync(cancellationToken);
        }
    }
}
