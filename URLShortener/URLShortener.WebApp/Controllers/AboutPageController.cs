using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;
using URLShortener.Services.WebApi.Interfaces;
using URLShortener.WebApp.Models.AboutPage;
using URLShortener.WebApp.Models.Account;

namespace URLShortener.WebApp.Controllers
{
    public class AboutPageController(IAboutPageService aboutPageService, ILogger<AboutPageController> logger, ITokenStore tokenStore) : Controller
    {
        private readonly IAboutPageService aboutPageService = aboutPageService ?? throw new ArgumentNullException(nameof(aboutPageService));
        private readonly ILogger<AboutPageController> logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ITokenStore tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var pageData = await aboutPageService.GetAboutPageInfoAsync();

                if (pageData != null)
                {
                    return View(new AboutPageViewModel()
                    {
                        Content = pageData.Content,
                        CreatedDate = pageData.CreatedDate,
                        LastModified = pageData.LastModified,
                        LastModifiedBy = pageData.LastModifiedBy,
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during About Page Data fetch: {Message}", ex.Message);
            }

            return View(new AboutPageViewModel()
            {
                Content = "Error loading the content.",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
            });
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AboutPageViewModel aboutPageViewModel)
        {
            _ = this.ModelState.IsValid;

            try
            {
                var userId = tokenStore.UserId;

                if (userId != null)
                {
                    await aboutPageService.UpdateAsync(aboutPageViewModel.Content, int.Parse(userId));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during About Page Data update: {Message}", ex.Message);
            }

            return RedirectToAction("Index", "AboutPage");
        }
    }
}
