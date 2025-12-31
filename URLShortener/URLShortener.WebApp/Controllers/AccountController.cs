using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;
using URLShortener.Services.WebApi.Interfaces;
using URLShortener.WebApp.Models;
using URLShortener.WebApp.Models.Account;

namespace URLShortener.WebApp.Controllers
{
    [Route("Account")]
    public class AccountController(ILogger<AccountController> logger, IAuthService authService, ITokenStore tokenStore) : Controller
    {
        private readonly ILogger<AccountController> logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IAuthService authService = authService ?? throw new ArgumentNullException(nameof(authService));
        private readonly ITokenStore tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));

        [HttpGet]
        [Route("Login")]
        [AllowAnonymous]
        public Task<ViewResult> Login(Uri? returnUrl = null)
        {
            returnUrl ??= new Uri("/", UriKind.Relative);

            _ = ModelState.IsValid;

            return Task.FromResult(
                View(new LoginViewModel
                {
                    ReturnUrl = returnUrl,
                })
            );
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (loginViewModel == null)
            {
                logger.LogError("Login view model is null.");
                throw new ArgumentNullException(nameof(loginViewModel));
            }

            if (this.ModelState.IsValid)
            {
                return this.LoginInternal(loginViewModel);
            }

            logger.LogError("Login view model is invalid.");
            return Task.FromResult<IActionResult>(this.View(loginViewModel));
        }

        [HttpPost]
        [Route("Logout")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated == true)
                {
                    this.tokenStore.Clear();
                    await HttpContext.SignOutAsync();
                }

                return this.RedirectToAction("Index", "AboutPage");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during logout: {Message}", ex.Message);
            }

            return RedirectToAction("Index", "AboutPage");
        }

        private async Task<IActionResult> LoginInternal(LoginViewModel loginViewModel)
        {
            try
            {
                await HttpContext.SignOutAsync("Cookies");

                var userDto = new UserDto
                {
                    Username = loginViewModel.UserName,
                    Password = loginViewModel.Password
                };

                TokenResponseDto tokenResult;

                try
                {
                    tokenResult = await authService.LoginAsync(userDto)
                        ?? throw new InvalidOperationException("Token was not returned.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Auth service error during login.");
                    ModelState.AddModelError(string.Empty,
                        "Unable to connect to authentication service. Please try again later.");
                    return View(loginViewModel);
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(tokenResult.AccessToken);

                var userId = jwt.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    logger.LogError("UserId claim not found in token.");
                    ModelState.AddModelError(string.Empty, "Invalid authentication token.");
                    return View(loginViewModel);
                }

                tokenStore.Save(tokenResult, userId);

                var claimsIdentity = new ClaimsIdentity(
                    jwt.Claims,
                    "Cookies"
                );

                var principal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    "Cookies",
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = loginViewModel.RememberMe,
                    });

                logger.LogInformation("User {Username} logged in successfully.", loginViewModel.UserName);

                return RedirectToAction("Index", "AboutPage");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login: {Message}", ex.Message);
            }

            return RedirectToAction("Index", "AboutPage");
        }
    }
}
