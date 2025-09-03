using FluentValidation;

namespace Order.Application.Order.Commands.DeleteOrder
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Sipari� ID'si bo� olamaz.");
        }
    }
}