using URLShortener.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddURLShortenerServices(builder.Configuration);

var app = builder.Build();

app.UseURLShortenerMiddleware();
app.MapURLShortenerRoutes();

await app.RunAsync();
