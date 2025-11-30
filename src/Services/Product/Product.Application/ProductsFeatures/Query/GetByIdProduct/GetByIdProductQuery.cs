using MediatR;
using Product.Domain.Entities;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Query.GetByIdProduct;

public sealed record GetByIdProductQuery(Guid Id) : IRequest<BaseResponse<ProductBase>>;
