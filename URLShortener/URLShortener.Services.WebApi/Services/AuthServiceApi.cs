using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;

namespace URLShortener.Services.WebApi.Services
{
    public class AuthServiceApi(HttpClient httpClient, ILogger<AuthServiceApi> logger) : IAuthService
    {
        private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly ILogger<AuthServiceApi> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            if (request == null)
            {
                logger.LogError("UserDto is empty.");
                return Task.FromResult<TokenResponseDto?>(null);
            }

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                logger.LogError("Login is empty.");
                return Task.FromResult<TokenResponseDto?>(null);
            }

            return LoginInternalAsync(request);
        }

        public Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            if (request == null)
            {
                logger.LogError("RefreshTokenRequestDto is empty.");
                return Task.FromResult<TokenResponseDto?>(null);
            }

            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                logger.LogError("UserId is empty.");
                return Task.FromResult<TokenResponseDto?>(null);
            }

            return RefreshTokensInteranlAsync(request);
        }

        private async Task<TokenResponseDto?> LoginInternalAsync(UserDto request)
        {
            try
            {
                using var response = await this.httpClient.PostAsJsonAsync("login", request);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
                    {
                        return tokenResponse;
                    }

                    logger.LogWarning("Api returned null.");
                    return null;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning("Error: {ErrorContent}", errorContent);
                return null;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An HTTP request exception occurred while logging in.");
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "An Json request exception occurred while logging in.");
            }

            return null;
        }

        private async Task<TokenResponseDto?> RefreshTokensInteranlAsync(RefreshTokenRequestDto request)
        {
            try
            {
                using var response = await this.httpClient.PostAsJsonAsync("refresh-token", request);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

                    if (tokenResponse != null &&
                        !string.IsNullOrEmpty(tokenResponse.AccessToken) &&
                        !string.IsNullOrEmpty(tokenResponse.RefreshToken))
                    {
                        return tokenResponse;
                    }

                    logger.LogWarning("Api returned null.");
                    return null;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning("Error: {ErrorContent}", errorContent);
                return null;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An HTTP request exception occurred while refreshing the token.");
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "An Json request exception occurred while refreshing the token.");
            }

            return null;
        }
    }
}
