using MediatR;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Command.UpdatePrice;

public sealed record UpdatePriceCommand(Guid ProductId, decimal NewPrice) : IRequest<BaseResponse>;
