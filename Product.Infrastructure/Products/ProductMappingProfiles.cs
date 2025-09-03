using AutoMapper;
using Product.Application.ProductsFeatures.Command.CreateProduct;
using Product.Application.ProductsFeatures.Command.UpdateProduct;
using Product.Application.ProductsFeatures.Query.GetByIdProduct;
using Product.Domain.Entities;

namespace Product.Infrastructure.Products
{
    public class ProductMappingProfiles : Profile
    {
        public ProductMappingProfiles()
        {

            CreateMap<CreateProductCommand, ProductBase>().ReverseMap();
            CreateMap<UpdateProductCommand, ProductBase>().ReverseMap();   
            CreateMap<GetByIdProductQuery, ProductBase>().ReverseMap();


        }
    }
}
