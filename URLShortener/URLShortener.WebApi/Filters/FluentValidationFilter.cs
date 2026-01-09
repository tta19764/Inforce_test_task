using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace URLShortener.WebApi.Filters
{
    public sealed class FluentValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if (validator is null) continue;

                var validationContext = new ValidationContext<object>(arg);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    context.Result = new BadRequestObjectResult(new
                    {
                        error = "VALIDATION_FAILED",
                        errors
                    });

                    return;
                }
            }

            await next();
        }
    }
}
