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
    public class URLShortenerUrlControllerTests
    {
        private readonly Mock<IUrlService> urlServiceMock = new();

        private UrlController CreateController(int? userId = null)
        {
            var controller = new UrlController(urlServiceMock.Object);

            if (userId.HasValue)
            {
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(
                            new ClaimsIdentity(
                            [
                                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                            ], "test"))
                    }
                };
            }

            return controller;
        }

        [Fact]
        public async Task GetUrl_ExistingId_ShouldReturnOk()
        {
            // Arrange
            var model = CreateUrlModel();
            urlServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(model);

            var controller = CreateController();

            // Act
            var result = await controller.GetUrl(1);

            // Assert
            result.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetUrl_NotFound_ShouldReturnNotFound()
        {
            // Arrange
            urlServiceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync((UrlModel?)null);

            var controller = CreateController();

            // Act
            var result = await controller.GetUrl(1);

            // Assert
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetUrl_Exception_ShouldReturnBadRequest()
        {
            // Arrange
            urlServiceMock.Setup(s => s.GetByIdAsync(1))
                .ThrowsAsync(new InvalidOperationException());

            var controller = CreateController();

            // Act
            var result = await controller.GetUrl(1);

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetCount_ShouldReturnOk()
        {
            // Arrande
            urlServiceMock.Setup(s => s.GetCount()).ReturnsAsync(5);

            var controller = CreateController();

            // Act
            var result = await controller.GetCount();

            // Assert
            result.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetUrls_Empty_ShouldReturnEmptyList()
        {
            // Arrange
            urlServiceMock.Setup(s => s.GetAllAsync(0, 0))
                .ReturnsAsync([]);

            var controller = CreateController();

            // Act
            var result = await controller.GetUrls();

            // Assert
            var ok = result.Result.ShouldBeOfType<OkObjectResult>();
            ((IEnumerable<object>)ok.Value!).Any().ShouldBeFalse();
        }

        [Fact]
        public async Task GetUrlsPaginated_ShouldReturnOk()
        {
            // Arrange
            urlServiceMock.Setup(s => s.GetAllAsync(1, 10))
                .ReturnsAsync([CreateUrlModel()]);

            var controller = CreateController();

            // Act
            var result = await controller.GetUrlsPaginated(1, 10);

            // Assert
            result.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AddUrl_NoUserClaim_ShouldReturnUnauthorized()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.AddUrl("https://test.com");

            // Assert
            result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task AddUrl_ValidUser_ShouldReturnOk()
        {
            // Arrange
            urlServiceMock.Setup(s => s.AddAsync(It.IsAny<UrlModel>()))
                .ReturnsAsync(CreateUrlModel());

            var controller = CreateController(userId: 1);

            // Act
            var result = await controller.AddUrl("https://test.com");

            // Assert
            result.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteUrl_NoUserClaim_ShouldReturnUnauthorized()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.DeleteUrl(1);

            // Assert
            result.ShouldBeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task DeleteUrl_ValidUser_ShouldReturnOk()
        {
            // Arrange
            urlServiceMock.Setup(s => s.DeleteAsync(1, 1))
                .Returns(Task.CompletedTask);

            var controller = CreateController(userId: 1);

            // Act
            var result = await controller.DeleteUrl(1);

            // Assert
            result.ShouldBeOfType<OkObjectResult>();
        }

        private static UrlModel CreateUrlModel()
        {
            return new UrlModel(1)
            {
                OriginalUrl = "https://example.com",
                ShortenedUrl = "abc123",
                CreatorId = 1,
                CreatorNickName = "tester"
            };
        }
    }
}
