using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;
using URLShortener.Services.Interfaces;
using URLShortener.Services.WebApi.Interfaces;
using URLShortener.Services.WebApi.Services;
using URLShortener.WebApp.Handlers;

namespace URLShortener.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddURLShortenerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";

                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;

                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = configuration["AppSettings:Issuer"],
                        ValidAudience = configuration["AppSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!)
                        ),

                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["access_token"];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddRazorPages();

            services.AddControllersWithViews();

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

            services.AddHttpContextAccessor();

            services
                .AddTransient<ITokenStore, CookieTokenStore>()
                .AddTransient<AuthTokenHandler>();

            services.AddHttpClient<IAboutPageService, AboutPageServiceWebApi>((provider, client) =>
            {
                var apiUrl = configuration["ApiSettings:ApiBaseUrl"];
                var controllerUrl = configuration["ApiSettings:ApiAboutPageController"];
                if (!string.IsNullOrWhiteSpace(apiUrl))
                {
                    client.BaseAddress = new Uri(apiUrl + controllerUrl);
                }
            })
            .AddHttpMessageHandler<AuthTokenHandler>();

            services.AddHttpClient<IAuthService, AuthServiceApi>((provider, client) =>
            {
                var apiUrl = configuration["ApiSettings:ApiBaseUrl"];
                var controllerUrl = configuration["ApiSettings:ApiAuthController"];

                client.BaseAddress = new Uri(apiUrl + controllerUrl);
            });

            return services;
        }
    }
}
