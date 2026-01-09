using FluentValidation;
using URLShortener.WebApi.Models.Dtos.Update;

namespace URLShortener.WebApi.Validators
{
    public sealed class UpdateAboutPageDtoValidator : AbstractValidator<UpdateAboutPageDto>
    {
        public UpdateAboutPageDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");
        }
    }
}
