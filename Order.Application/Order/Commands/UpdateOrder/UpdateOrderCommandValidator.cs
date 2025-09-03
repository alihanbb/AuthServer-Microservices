using FluentValidation;

namespace Order.Application.Order.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Sipariþ ID'si boþ olamaz.");

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