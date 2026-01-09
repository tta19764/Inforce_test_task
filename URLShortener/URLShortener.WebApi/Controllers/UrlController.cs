using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;
using URLShortener.WebApi.Models.Dtos.Create;
using URLShortener.WebApi.Models.Dtos.Read;

namespace URLShortener.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController(IUrlService urlService) : ControllerBase
    {
        private readonly IUrlService urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));

        [HttpGet("{id:min(1)}")]
        [Authorize]
        public async Task<ActionResult<UrlDto>> GetUrl(int id)
        {
            try
            {
                var url = await this.urlService.GetByIdAsync(id);

                if(url is null)
                {
                    return NotFound($"Url with Id: {id} was not found.");
                }

                var urlDto = MapToUrlDto(url);
                return Ok(urlDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An unexpected error occurred while retrieving the url: {ex}");
            }
        }

        [HttpGet("Count")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetCount()
        {
            try
            {
                var count = await this.urlService.GetCount();

                return Ok(count);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while retrieving the urls count: {ex}");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UrlDto>>> GetUrls()
        {
            try
            {
                var urls = await this.urlService.GetAllAsync();

                if (urls is null || !urls.Any())
                {
                    return Ok(new List<UrlDto>());
                }

                var urlDtos = urls.Select(MapToUrlDto);

                return Ok(urlDtos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while retrieving all urls: {ex}");
            }
        }

        [HttpGet("{pageNumber:min(1)}/{pageSize:min(1)}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UrlDto>>> GetUrlsPaginated(int pageNumber, int pageSize)
        {
            try
            {
                var urls = await this.urlService.GetAllAsync(pageNumber, pageSize);

                if (urls is null || !urls.Any())
                {
                    return Ok(new List<UrlDto>());
                }

                var urlDtos = urls.Select(MapToUrlDto);

                return Ok(urlDtos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while retrieving paginated urls: \n{ex}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UrlDto>> AddUrl([FromBody] CreateUrlDto url)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var newUrl = new UrlModel()
                {
                    OriginalUrl = url.OriginalUrl,
                    CreatorId = userId,
                };

                var result = await this.urlService.AddAsync(newUrl);
                if (result is null)
                {
                    return BadRequest("Unable to add url.");
                }

                return Ok(MapToUrlDto(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while adding url: \n{ex}");
            }
        }

        [HttpDelete("{id:min(1)}")]
        [Authorize]
        public async Task<ActionResult> DeleteUrl(int id)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                await this.urlService.DeleteAsync(id, userId);

                return Ok("The delete operation was successful.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while deleting the URL: \n{ex}");
            }
        }

        private static UrlDto MapToUrlDto(UrlModel model)
        {
            return new UrlDto
            {
                Id = model.Id,
                OriginalUrl = model.OriginalUrl,
                ShortUrl = model.ShortenedUrl,
                CreatorId = model.CreatorId,
                CreatedAt = model.CreatedAt,
                CreatorNickname = model.CreatorNickName,
                ClickCount = model.ClickCount,
            };
        }
    }
}
