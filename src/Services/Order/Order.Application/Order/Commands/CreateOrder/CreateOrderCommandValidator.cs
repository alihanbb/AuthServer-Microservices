using FluentValidation;

namespace Order.Application.Order.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Müþteri adý boþ olamaz.")
                .MaximumLength(100).WithMessage("Müþteri adý 100 karakterden uzun olamaz.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Toplam tutar 0'dan büyük olmalýdýr.");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Sipariþ tarihi boþ olamaz.");
        }
    }
}