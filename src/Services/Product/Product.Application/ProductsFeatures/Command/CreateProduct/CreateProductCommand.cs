using MediatR;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.CreateProduct;

public sealed record CreateProductCommand(string Name,
    string Description, decimal Price, int StockQuantity) : IRequest<BaseResponse>;
