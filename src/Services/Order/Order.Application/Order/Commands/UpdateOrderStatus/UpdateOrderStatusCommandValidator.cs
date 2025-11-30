using FluentValidation;

namespace Order.Application.Order.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Sipariþ ID'si boþ olamaz.");
        }
    }
}