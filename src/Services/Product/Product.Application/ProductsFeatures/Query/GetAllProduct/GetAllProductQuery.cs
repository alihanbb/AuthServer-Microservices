using MediatR;
using Product.Domain.Entities;
using SharedLibrary.Common;

namespace Product.Application.ProductsFeatures.Query.GetAllProduct;

public sealed record GetAllProductQuery() : IRequest<BaseResponse<List<ProductBase>>>;

