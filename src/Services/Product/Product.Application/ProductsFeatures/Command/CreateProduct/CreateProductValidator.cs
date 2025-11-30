using FluentValidation;

namespace Product.Application.ProductsFeatures.Command.CreateProduct
{
    public class CreateProductValidator  : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator() 
        { 
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı gereklidir.")
                .MaximumLength(50).WithMessage("Ürün adı 50 karakteri geçmemelidir.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Ürün açıklaması gereklidir.")
                .MaximumLength(250).WithMessage("Ürün açıklaması 250 karakteri geçmemelidir.");
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Ürün fiyatı sıfırdan büyük olmalıdır.");
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı sıfır veya daha büyük olmalıdır.");
        }
    }
}
