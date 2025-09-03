using FluentValidation;

namespace Customer.Application.Customer.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("M��teri ID'si bo� olamaz.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad bo� olamaz.")
                .MaximumLength(50).WithMessage("Ad 50 karakterden uzun olamaz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad bo� olamaz.")
                .MaximumLength(50).WithMessage("Soyad 50 karakterden uzun olamaz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi bo� olamaz.")
                .EmailAddress().WithMessage("Ge�erli bir e-posta adresi giriniz.")
                .MaximumLength(100).WithMessage("E-posta adresi 100 karakterden uzun olamaz.");
        }
    }
}