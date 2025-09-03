using FluentValidation;

namespace Order.Application.Order.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("M��teri ad� bo� olamaz.")
                .MaximumLength(100).WithMessage("M��teri ad� 100 karakterden uzun olamaz.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Toplam tutar 0'dan b�y�k olmal�d�r.");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Sipari� tarihi bo� olamaz.");
        }
    }
}