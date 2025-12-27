using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;
using URLShortener.WebApi.Controllers;

namespace URLShortener.Tests.Controllers
{
    public class URLShortenerAboutPageControllerTests
    {
        public class AboutPageControllerTests
        {
            private readonly Mock<IAboutPageService> aboutServiceMock = new();

            private AboutPageController CreateController(
                bool authenticated = false,
                int? userId = null)
            {
                var controller = new AboutPageController(aboutServiceMock.Object);

                if (authenticated)
                {
                    var claims = userId.HasValue
                        ? new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) }
                        : [];

                    controller.ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(
                                new ClaimsIdentity(claims, "test"))
                        }
                    };
                }

                return controller;
            }

            [Fact]
            public async Task GetAboutPage_ShouldReturnOk()
            {
                // Arrange
                aboutServiceMock.Setup(s => s.GetAboutPageInfoAsync())
                    .ReturnsAsync(CreateAboutModel());

                var controller = CreateController();

                // Act
                var result = await controller.GetAboutPage();

                // Assert
                result.Result.ShouldBeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task GetAboutPage_NotFound_ShouldReturnNotFound()
            {
                // Arrange
                aboutServiceMock.Setup(s => s.GetAboutPageInfoAsync())
                    .ReturnsAsync((AboutPageModel?)null);

                var controller = CreateController();

                // Act
                var result = await controller.GetAboutPage();

                // Assert
                result.Result.ShouldBeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task GetAboutPage_Exception_ShouldReturnBadRequest()
            {
                // Arrange
                aboutServiceMock.Setup(s => s.GetAboutPageInfoAsync())
                    .ThrowsAsync(new InvalidOperationException());

                var controller = CreateController();

                // Act
                var result = await controller.GetAboutPage();

                // Assert
                result.Result.ShouldBeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task UpdateAboutPage_NotAuthenticated_ShouldReturnUnauthorized()
            {
                // Arrange
                var controller = CreateController(authenticated: false);

                // Act
                var result = await controller.UpdateAboutPage("content");

                // Assert
                result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
            }

            [Fact]
            public async Task UpdateAboutPage_NoUserIdClaim_ShouldReturnUnauthorized()
            {
                // Arrange
                var controller = CreateController(authenticated: true);

                // Act
                var result = await controller.UpdateAboutPage("content");

                // Assert
                result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
            }

            [Fact]
            public async Task UpdateAboutPage_ValidUser_ShouldReturnOk()
            {
                // Arrange
                aboutServiceMock.Setup(s => s.UpdateAsync("content", 1))
                    .ReturnsAsync(CreateAboutModel());

                var controller = CreateController(authenticated: true, userId: 1);

                // Act
                var result = await controller.UpdateAboutPage("content");

                // Assert
                result.Result.ShouldBeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task UpdateAboutPage_Exception_ShouldReturnBadRequest()
            {
                // Arrange
                aboutServiceMock.Setup(s => s.UpdateAsync("content", 1))
                    .ThrowsAsync(new InvalidOperationException());

                var controller = CreateController(authenticated: true, userId: 1);

                // Act
                var result = await controller.UpdateAboutPage("content");

                // Assert
                result.Result.ShouldBeOfType<BadRequestObjectResult>();
            }

            private static AboutPageModel CreateAboutModel()
            {
                return new AboutPageModel
                {
                    Content = "About content",
                    CreatedDate = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    LastModifiedById = 1,
                    LastModifiedBy = "admin"
                };
            }
        }
    }
}
