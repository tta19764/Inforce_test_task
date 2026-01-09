

using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using URLShortener.Services.Database.Data;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Database.Repositories;
using URLShortener.Services.Database.Servicies;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Services;
using URLShortener.WebApi.Filters;

namespace URLShortener.WebApi.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddURLShortenerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UrlShortenerDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("UrlShortenerConnection"),
                    b => b.MigrationsAssembly("URLShortener.WebApi"));
            });

            services
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<IUrlRepository, UrlRepository>()
                .AddTransient<IAboutPageRepository, AboutPageRepository>();

            services
                .AddTransient<IUserService, UserService>()
                .AddTransient<IPasswordHasher, PasswordHasher>()
                .AddTransient<IUrlShorteningService, UrlShorteningService>()
                .AddTransient<IAuthService, AuthService>()
                .AddTransient<IAboutPageService, AboutPageService>()
                .AddTransient<IUrlService, UrlService>();

            services.AddControllers(options =>
            {
                options.Filters.Add<FluentValidationFilter>();
            });
            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "URLShortener API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer eyJhbGciOiJI...\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
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
                        Encoding.UTF8.GetBytes(configuration["AppSettings:Token"] ?? "SomeSuperSecureKey")),
                    ClockSkew = TimeSpan.FromMinutes(5),
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async ctx =>
                    {
                        ctx.HandleResponse();

                        var response = ctx.Response;
                        response.StatusCode = 401;
                        response.ContentType = "application/json";

                        string message;
                        string errorCode;

                        if (ctx.AuthenticateFailure is SecurityTokenExpiredException)
                        {
                            message = "Token has expired";
                            errorCode = "TOKEN_EXPIRED";
                            response.Headers.Append("Token-Expired", "true");
                        }
                        else if (ctx.AuthenticateFailure is SecurityTokenInvalidSignatureException)
                        {
                            message = "Invalid token signature";
                            errorCode = "INVALID_SIGNATURE";
                        }
                        else if (ctx.AuthenticateFailure is SecurityTokenValidationException)
                        {
                            message = "Invalid token";
                            errorCode = "INVALID_TOKEN";
                        }
                        else
                        {
                            message = "Authentication failed";
                            errorCode = "AUTH_FAILED";
                        }

                        var result = new
                        {
                            error = errorCode,
                            message,
                            timestamp = DateTime.UtcNow,
                        };

                        await response.WriteAsync(JsonSerializer.Serialize(result));
                    },
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            services.AddAuthorization();

            return services;
        }
    }
}
