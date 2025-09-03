using MediatR;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<BaseResponse>;
