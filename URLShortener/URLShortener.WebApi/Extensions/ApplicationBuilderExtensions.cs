using URLShortener.WebApi.Middleware;

namespace URLShortener.WebApi.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        public static WebApplication UseURLShortenerMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("FrontendPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static WebApplication MapURLShortenerRoutes(this WebApplication app)
        {
            app.MapControllers();

            return app;
        }
    }
}
