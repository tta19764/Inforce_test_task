using FluentValidation;
using URLShortener.WebApi.Models.Dtos;

namespace URLShortener.WebApi.Validators
{
    public sealed class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");

            RuleFor(x => x.Nickname)
                .NotEmpty().WithMessage("Nickname is required.");

            RuleFor(x => x.AccountType)
                .IsInEnum().WithMessage("AccountType must be a valid enum value.");
        }
    }
}
