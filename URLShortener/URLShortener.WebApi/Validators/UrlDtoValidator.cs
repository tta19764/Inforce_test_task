using FluentValidation;
using URLShortener.WebApi.Models.Dtos.Create;

namespace URLShortener.WebApi.Validators
{
    public sealed class UrlDtoValidator : AbstractValidator<CreateUrlDto>
    {
        public UrlDtoValidator()
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("OriginalUrl is required.");
        }
    }
}
