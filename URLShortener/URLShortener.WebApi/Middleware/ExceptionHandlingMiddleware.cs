namespace URLShortener.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
        private readonly ILogger<ExceptionHandlingMiddleware> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, exception.Message),
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "An error occurred while processing your request")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode,
                message,
                details = context.RequestServices
                    .GetService<IWebHostEnvironment>()?
                    .IsDevelopment() == true
                        ? exception.ToString()
                        : null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
