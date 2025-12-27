using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Interfaces;
using URLShortener.WebApi.Models.Dtos.Read;

namespace URLShortener.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutPageController(IAboutPageService aboutPageService) : ControllerBase
    {
        private readonly IAboutPageService aboutPageService = aboutPageService ?? throw new ArgumentNullException(nameof(aboutPageService));

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<AboutPageDto>> GetAboutPage()
        {
            try
            {
                var page = await this.aboutPageService.GetAboutPageInfoAsync();

                if (page == null)
                {
                    return NotFound();
                }

                return Ok(
                    new AboutPageDto()
                    {
                        Content = page.Content,
                        CreatedDate = page.CreatedDate,
                        LastModified = page.LastModified,
                        LastModifiedById = page.LastModifiedById,
                        LastModifiedBy = page.LastModifiedBy,
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while retrieving the AboutPage: \n{ex}");
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<AboutPageDto>> UpdateAboutPage([FromBody] string content)
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

                var result = await this.aboutPageService.UpdateAsync(content, userId);

                if(result == null)
                {
                    return BadRequest("Unable to update url.");
                }

                return Ok(
                    new AboutPageDto()
                    {
                        Content = result.Content,
                        CreatedDate = result.CreatedDate,
                        LastModified = result.LastModified,
                        LastModifiedById = result.LastModifiedById,
                        LastModifiedBy = result.LastModifiedBy,
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while updating the AboutPage: \n{ex}");
            }
        }
    }
}
