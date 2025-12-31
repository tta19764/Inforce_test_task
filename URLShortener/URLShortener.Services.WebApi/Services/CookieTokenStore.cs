using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.JWT;
using URLShortener.Services.WebApi.Interfaces;

namespace URLShortener.Services.WebApi.Services
{
    public sealed class CookieTokenStore(IHttpContextAccessor httpContextAccessor) : ITokenStore
    {
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string UserIdKey = "user_id";

        private string? accessToken;
        private string? refreshToken;
        private string? userId;

        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        private HttpContext HttpContext =>
            httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");

        public string? AccessToken =>
            accessToken ?? HttpContext.Request.Cookies[AccessTokenKey];

        public string? RefreshToken =>
            refreshToken ?? HttpContext.Request.Cookies[RefreshTokenKey];

        public string? UserId =>
            userId ?? HttpContext.Request.Cookies[UserIdKey];

        public void Save(TokenResponseDto tokens, string userId)
        {
            var options = CreateCookieOptions();

            this.accessToken = tokens.AccessToken;
            this.refreshToken = tokens.RefreshToken;
            this.userId = userId;

            HttpContext.Response.Cookies.Append(AccessTokenKey, tokens.AccessToken, options);
            HttpContext.Response.Cookies.Append(RefreshTokenKey, tokens.RefreshToken, options);
            HttpContext.Response.Cookies.Append(UserIdKey, userId, options);
        }

        public void Clear()
        {
            accessToken = null;
            refreshToken = null;
            userId = null;

            HttpContext.Response.Cookies.Delete(AccessTokenKey);
            HttpContext.Response.Cookies.Delete(RefreshTokenKey);
            HttpContext.Response.Cookies.Delete(UserIdKey);
        }

        private static CookieOptions CreateCookieOptions() =>
            new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
    }

}
