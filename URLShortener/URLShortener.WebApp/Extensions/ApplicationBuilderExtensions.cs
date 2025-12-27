namespace URLShortener.WebApp.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        public static WebApplication UseURLShortenerMiddleware(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static WebApplication MapURLShortenerRoutes(this WebApplication app)
        {
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AboutPage}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "root",
                pattern: "",
                defaults: new { controller = "AboutPage", action = "Index" });

            app.MapRazorPages();

            return app;
        }
    }
}
