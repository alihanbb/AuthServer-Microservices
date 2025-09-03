using FluentValidation;

namespace Order.Application.Order.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Sipari� ID'si bo� olamaz.");

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