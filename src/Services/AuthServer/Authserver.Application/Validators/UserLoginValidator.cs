using Authserver.Application.DTOs;
using FluentValidation;

namespace Authserver.Application.Validators
{
    public class UserLoginValidator : AbstractValidator<LoginRequestDto>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Username)
                 .NotEmpty();



            RuleFor(x => x.Password)
                .NotEmpty();
       
        }
    }
}
