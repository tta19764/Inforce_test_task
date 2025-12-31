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

        private const string RETRYKEY = "AuthRetry";
        private static readonly HttpRequestOptionsKey<bool> RetryKey =
            new(RETRYKEY);

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            AttachAccessToken(request);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (request.Options.TryGetValue(RetryKey, out _))
            {
                return response;
            }

            logger.LogInformation("401 received. Attempting token refresh.");

            var refreshed = await TryRefreshTokenAsync();
            if (!refreshed)
            {
                logger.LogWarning("Token refresh failed.");
                return response;
            }

            var retryRequest = await CloneHttpRequestAsync(request);
            retryRequest.Options.TryAdd(RETRYKEY, true);

            AttachAccessToken(retryRequest);

            return await base.SendAsync(retryRequest, cancellationToken);
        }

        private void AttachAccessToken(HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(tokenStore.AccessToken))
            {
                return;
            }

            request.Headers.Remove("Authorization");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
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

        private static async Task<HttpRequestMessage> CloneHttpRequestAsync(
            HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(
                request.Method,
                request.RequestUri);

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(content);

                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clone;
        }
    }
}
