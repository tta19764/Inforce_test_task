using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;
using URLShortener.WebApi.Models.Dtos.Read;
using URLShortener.WebApi.Models.Dtos.Update;

namespace URLShortener.Services.WebApi.Services
{
    public class AboutPageServiceWebApi(HttpClient httpClient, ILogger<AboutPageServiceWebApi> logger) : IAboutPageService
    {
        private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly ILogger<AboutPageServiceWebApi> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<AboutPageModel> GetAboutPageInfoAsync()
        {
            try
            {
                using var response = await this.httpClient.GetAsync(this.httpClient.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    var pageInfo = await response.Content.ReadFromJsonAsync<AboutPageDto>();
                    if (pageInfo != null)
                    {
                        return new AboutPageModel()
                        {
                            Content = pageInfo.Content,
                            CreatedDate = pageInfo.CreatedDate,
                            LastModified = pageInfo.LastModified,
                            LastModifiedBy = pageInfo.LastModifiedBy,
                            LastModifiedById = pageInfo.LastModifiedById,
                        };
                    }

                    logger.LogWarning("Get about page returned null.");
                    return new AboutPageModel()
                    {
                        Content = string.Empty
                    };
                }

                throw new HttpRequestException($"Failed to retrieve about page: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An HTTP request exception occurred while retrieving about page info.");
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "A Json exception occurred while retrieving about page info.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while retrieving about page info.");
            }

            return new AboutPageModel()
            {
                Content = string.Empty
            };
        }

        public Task<AboutPageModel> UpdateAsync(string content, int userId)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

            return UpdateInternalAsync(content);
        }

        private async Task<AboutPageModel> UpdateInternalAsync(string content)
        {
            try
            {
                using var response = await this.httpClient.PutAsJsonAsync(this.httpClient.BaseAddress, new UpdateAboutPageDto(content));

                if (response.IsSuccessStatusCode)
                {
                    var pageInfo = await response.Content.ReadFromJsonAsync<AboutPageDto>();
                    if (pageInfo != null)
                    {
                        return new AboutPageModel()
                        {
                            Content = pageInfo.Content,
                            CreatedDate = pageInfo.CreatedDate,
                            LastModified = pageInfo.LastModified,
                            LastModifiedBy = pageInfo.LastModifiedBy,
                            LastModifiedById = pageInfo.LastModifiedById,
                        };
                    }

                    logger.LogWarning("Update about page returned null.");
                    return new AboutPageModel()
                    {
                        Content = string.Empty
                    };
                }

                throw new HttpRequestException($"Failed to update about page: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An HTTP request exception occurred while updating about page info.");
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "A Json exception occurred while updating about page info.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while updating about page info.");
            }

            return new AboutPageModel()
            {
                Content = string.Empty
            };
        }
    }
}
