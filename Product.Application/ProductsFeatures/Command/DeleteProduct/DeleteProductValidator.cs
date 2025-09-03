using FluentValidation;

namespace Product.Application.ProductsFeatures.Command.DeleteProduct
{
    public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("�r�n Id'si gereklidir.")
                .NotEqual(Guid.Empty).WithMessage("Ge�erli bir �r�n Id'si giriniz.");
        }
    }
}