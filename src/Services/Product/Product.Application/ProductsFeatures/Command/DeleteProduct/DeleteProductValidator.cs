using FluentValidation;

namespace Product.Application.ProductsFeatures.Command.DeleteProduct
{
    public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Ürün Id'si gereklidir.")
                .NotEqual(Guid.Empty).WithMessage("Geçerli bir ürün Id'si giriniz.");
        }
    }
}