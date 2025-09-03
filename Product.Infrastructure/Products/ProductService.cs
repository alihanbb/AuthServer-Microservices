using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Product.Application.ProductsFeatures.Command.CreateProduct;
using Product.Application.ProductsFeatures.Command.DeleteProduct;
using Product.Application.ProductsFeatures.Command.UpdateProduct;
using Product.Application.Services;
using Product.Domain.Entities;
using SharedLibrary.Common;
using SharedLibrary.Events;
using SharedLibrary.Messaging;

namespace Product.Infrastructure.Products
{
    public class ProductService : IProductService
    {
        private readonly ProductBaseDbContext _productBaseDbContext;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public ProductService(
            ProductBaseDbContext productBaseDbContext, 
            IMapper mapper,
            IEventPublisher eventPublisher)
        {
            _productBaseDbContext = productBaseDbContext;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<BaseResponse> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var createProduct = _mapper.Map<ProductBase>(command);
                createProduct.CreatedAt = DateTime.UtcNow;
                createProduct.UpdatedAt = DateTime.UtcNow;
                createProduct.IsDeleted = false;

                await _productBaseDbContext.Products.AddAsync(createProduct, cancellationToken);
                await _productBaseDbContext.SaveChangesAsync(cancellationToken);

                // Publish ProductCreated event
                var productCreatedEvent = new ProductCreatedEvent(
                    createProduct.Id,
                    createProduct.Name,
                    createProduct.Description,
                    createProduct.Price,
                    createProduct.StockQuantity,
                    createProduct.CreatedAt
                );
                
                await _eventPublisher.PublishAsync(productCreatedEvent, cancellationToken);

                return new CreatedResponse("Ürün başarıyla oluşturuldu", 201, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Ürün oluşturulurken hata oluştu: {ex.Message}", 500, false);
            }
        }

        public async Task<BaseResponse> DeleteProductAsync(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            try
            {   
                var deleteProduct = await _productBaseDbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
                
                if (deleteProduct is null)
                    return new ErrorResponse($"Ürün bulunamadı. ID: {command.Id} ", 404, false);

                if (deleteProduct.IsDeleted)
                    return new ErrorResponse($"Ürün zaten silinmiş durumda. ID: {command.Id}", 400, false);
                
                deleteProduct.IsDeleted = true;
                deleteProduct.DeletedAt = DateTime.UtcNow;
                deleteProduct.UpdatedAt = DateTime.UtcNow;
                
                await _productBaseDbContext.SaveChangesAsync(cancellationToken);

                // Publish ProductDeleted event
                var productDeletedEvent = new ProductDeletedEvent(
                    deleteProduct.Id,
                    deleteProduct.DeletedAt.Value
                );
                
                await _eventPublisher.PublishAsync(productDeletedEvent, cancellationToken);

                return new SuccessResponse($"Ürün başarıyla silindi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Ürün silinirken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse<List<ProductBase>>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var listProducts = await _productBaseDbContext.Products
                    .Where(p => !p.IsDeleted)
                    .ToListAsync(cancellationToken);
                return new SuccessResponse<List<ProductBase>>(listProducts, "Ürünler başarıyla listelendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse<List<ProductBase>>(null,$"Ürünler listelenirken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse<ProductBase?>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var anyProduct = await _productBaseDbContext.Products
               .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                if (anyProduct is null)
                    return new ErrorResponse<ProductBase?>(null, $"Ürün bulunamadı. ID: {id}", 404, false);
                var productDto = _mapper.Map<ProductBase>(anyProduct);
                return new SuccessResponse<ProductBase?>(productDto, "Ürün başarıyla bulundu.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse<ProductBase?>(null, $"Ürün bulunurken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse> UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var updateProduct = await _productBaseDbContext.Products.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                if (updateProduct is null)
                    return new ErrorResponse($"Ürün bulunamadı. ID: {command.Id}", 404, false);

                var oldPrice = updateProduct.Price;
                var oldStock = updateProduct.StockQuantity;
                
                var productDto = _mapper.Map(command, updateProduct);
                productDto.UpdatedAt = DateTime.UtcNow;
                productDto.IsDeleted = false;
                
                await _productBaseDbContext.SaveChangesAsync(cancellationToken);

                // Publish ProductUpdated event
                var productUpdatedEvent = new ProductUpdatedEvent(
                    productDto.Id,
                    productDto.Name,
                    productDto.Description,
                    productDto.Price,
                    productDto.StockQuantity,
                    productDto.UpdatedAt
                );
                
                await _eventPublisher.PublishAsync(productUpdatedEvent, cancellationToken);

                // If price changed, publish price change event
                if (oldPrice != productDto.Price)
                {
                    var priceChangedEvent = new ProductPriceChangedEvent(
                        productDto.Id,
                        oldPrice,
                        productDto.Price,
                        productDto.UpdatedAt
                    );
                    
                    await _eventPublisher.PublishAsync(priceChangedEvent, cancellationToken);
                }

                // If stock changed, publish stock update event
                if (oldStock != productDto.StockQuantity)
                {
                    var stockUpdatedEvent = new ProductStockUpdatedEvent(
                        productDto.Id,
                        oldStock,
                        productDto.StockQuantity,
                        productDto.UpdatedAt
                    );
                    
                    await _eventPublisher.PublishAsync(stockUpdatedEvent, cancellationToken);
                }

                return new SuccessResponse("Ürün başarıyla güncellendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Ürün güncellenirken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }

        public async Task<BaseResponse> UpdateProductPriceAsync(Guid productId, decimal newPrice, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productBaseDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
                if (product is null)
                    return new ErrorResponse($"Ürün bulunamadı. ID: {productId}", 404, false);
                
                var oldPrice = product.Price;
                product.Price = newPrice;
                product.UpdatedAt = DateTime.UtcNow;
                
                await _productBaseDbContext.SaveChangesAsync(cancellationToken);

                // Publish ProductPriceChanged event
                var priceChangedEvent = new ProductPriceChangedEvent(
                    product.Id,
                    oldPrice,
                    newPrice,
                    product.UpdatedAt
                );
                
                await _eventPublisher.PublishAsync(priceChangedEvent, cancellationToken);
                
                return new SuccessResponse("Ürün fiyatı başarıyla güncellendi.", 200, true);
            }
            catch (Exception ex)
            {
                return new ErrorResponse($"Ürün fiyatı güncellenirken hata oluştu: {ex.Message}. Stack Trace: {ex.StackTrace}", 500, false);
            }
        }
    }
}
