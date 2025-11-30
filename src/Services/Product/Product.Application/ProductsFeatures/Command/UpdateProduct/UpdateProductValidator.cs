using FluentValidation;

namespace Product.Application.ProductsFeatures.Command.UpdateProduct
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Ürün ID'si boş olamaz.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Ürün adı boş olamaz.");
                
            RuleFor(x => x.Description)
                .Length(2, 100)
                .WithMessage("Ürün adı 2-100 karakter arasında olmalıdır.");

            RuleFor(x => x.Description)
                .Length(0, 250)
                .WithMessage("Ürün açıklaması en fazla 250 karakter olabilir.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stok miktarı 0 veya daha büyük olmalıdır.");
                


        }
    }
}
