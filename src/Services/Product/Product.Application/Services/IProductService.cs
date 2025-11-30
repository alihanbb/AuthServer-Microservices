using Product.Application.ProductsFeatures.Command.CreateProduct;
using Product.Application.ProductsFeatures.Command.DeleteProduct;
using Product.Application.ProductsFeatures.Command.UpdateProduct;
using Product.Domain.Entities;
using SharedLibrary.Common;

namespace Product.Application.Services
{
    public interface IProductService
    {
        Task<BaseResponse> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken);
        Task<BaseResponse> DeleteProductAsync(DeleteProductCommand command, CancellationToken cancellationToken);
        Task<BaseResponse> UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken);
        Task<BaseResponse<List<ProductBase>>> GetAllProductsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<ProductBase?>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse> UpdateProductPriceAsync(Guid productId, decimal newPrice, CancellationToken cancellationToken);

    }
}
