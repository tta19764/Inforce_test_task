using System.Net.Http.Headers;
using System.Net;
using URLShortener.Services.WebApi.Interfaces;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Text.Json;
using System.Threading;

namespace URLShortener.WebApp.Handlers
{
    public sealed class AuthTokenHandler(
        ITokenStore tokenStore,
        IAuthService authService,
        ILogger<AuthTokenHandler> logger) : DelegatingHandler
    {
        private readonly ITokenStore tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        private readonly IAuthService authService = authService ?? throw new ArgumentNullException(nameof(authService));
        private readonly ILogger<AuthTokenHandler> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await AttachAccessToken(request);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task AttachAccessToken(HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(tokenStore.AccessToken))
                return;

            try
            {
                var jwt = new JwtSecurityTokenHandler()
                    .ReadJwtToken(tokenStore.AccessToken);

                var expUtc = jwt.ValidTo;

                if (expUtc <= DateTime.UtcNow.AddMinutes(1) && !await TryRefreshTokenAsync())
                {
                    logger.LogWarning("Token refresh failed.");
                    return;
                }

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Invalid access token format.");
                tokenStore.Clear();
            }
        }

        private async Task<bool> TryRefreshTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(tokenStore.RefreshToken) ||
                string.IsNullOrWhiteSpace(tokenStore.UserId))
            {
                tokenStore.Clear();
                return false;
            }

            var response = await authService.RefreshTokensAsync(
                new RefreshTokenRequestDto
                {
                    RefreshToken = tokenStore.RefreshToken,
                    UserId = tokenStore.UserId
                });

            if (response == null)
            {
                tokenStore.Clear();
                return false;
            }

            tokenStore.Save(response, tokenStore.UserId);
            return true;
        }
    }
}
