using FluentValidation;
using URLShortener.Services.JWT;

namespace URLShortener.WebApi.Validators
{
    public sealed class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("RefreshToken is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
