using FluentValidation;

namespace Customer.Application.Customer.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
    {
        public DeleteCustomerCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("M��teri ID'si bo� olamaz.");
        }
    }
}