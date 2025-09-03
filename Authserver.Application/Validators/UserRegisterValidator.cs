using Authserver.Application.DTOs;
using FluentValidation;

namespace Authserver.Application.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Kullanıcı adı gereklidir.")
                .Length(3, 20)
                .WithMessage("Kullanıcı adı 3 ile 20 karakter arasında olmalıdır.");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Şifre gereklidir.")
                .MinimumLength(6)
                .WithMessage("Şifre en az 6 karakter uzunluğunda olmalıdır.");
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-posta gereklidir.")
                .EmailAddress()
                .WithMessage("Geçersiz e-posta formatı.");
        }
    }
}
