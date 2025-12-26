using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Database.Servicies;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Services;

namespace URLShortener.Tests
{
    public class URLShortenerUrlServiceTests
    {
        private readonly IUrlShorteningService urlShorteningService;

        public URLShortenerUrlServiceTests()
        {
            urlShorteningService = new UrlShorteningService();
        }

        [Fact]
        public async Task AddAsync_ShuldGenerateShortCodeCallAddAync()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            var model = StaticMethods.CreateUrlModel();
            var entity = StaticMethods.CreateUrl(model.OriginalUrl, urlShorteningService.GenerateShortCode(model.OriginalUrl));

            repoMock.Setup(r => r.AddAsync(It.IsAny<Url>()))
                .ReturnsAsync(entity);

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act
            var result = await service.AddAsync(model);

            // Assert
            result.OriginalUrl.ShouldBe(model.OriginalUrl);
            result.ShortenedUrl.ShouldBe(urlShorteningService.GenerateShortCode(model.OriginalUrl));
            result.CreatorNickName.ShouldBe(model.CreatorNickName);
            repoMock.Verify(r => r.AddAsync(It.Is<Url>(url => url.OriginalURL == entity.OriginalURL)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShuldGenerateShortCodeCallUpdateAync()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            var model = StaticMethods.CreateUrlModel();
            var entity = StaticMethods.CreateUrl(model.OriginalUrl, urlShorteningService.GenerateShortCode(model.OriginalUrl));

            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Url>()))
                .ReturnsAsync(entity);

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act
            var result = await service.UpdateAsync(model);

            // Assert
            result.OriginalUrl.ShouldBe(model.OriginalUrl);
            result.ShortenedUrl.ShouldBe(urlShorteningService.GenerateShortCode(model.OriginalUrl));
            result.CreatorNickName.ShouldBe(model.CreatorNickName);
            repoMock.Verify(r => r.UpdateAsync(It.Is<Url>(url => url.OriginalURL == entity.OriginalURL)), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShuldReturnFoundValueCodeCallGetByIdAync()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            var model = StaticMethods.CreateUrlModel();
            var entity = StaticMethods.CreateUrl(model.OriginalUrl, urlShorteningService.GenerateShortCode(model.OriginalUrl));

            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(entity);

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act
            var result = await service.GetByIdAsync(model.Id);

            // Assert
            result!.OriginalUrl.ShouldBe(model.OriginalUrl);
            result.ShortenedUrl.ShouldBe(urlShorteningService.GenerateShortCode(model.OriginalUrl));
            result.CreatorNickName.ShouldBe(model.CreatorNickName);
            repoMock.Verify(r => r.GetByIdAsync(model.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShuldCallDeleteAync()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            int id = 1;
            
            repoMock.Setup(r => r.DeleteAsync(It.IsAny<int>()));

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act
            await service.DeleteAsync(id);

            // Assert
            repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShuldThrowInvalidOperationExceptionWhenUrlExists()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            var model = StaticMethods.CreateUrlModel();

            repoMock.Setup(r => r.AddAsync(It.Is<Url>(u => u.OriginalURL == model.OriginalUrl)))
                .ThrowsAsync(new DbUpdateException("UNIQUE"));

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => service.AddAsync(model), "The service must throw InvalidOperationException.");
        }

        [Fact]
        public async Task UpdateAsync_ShuldThrowInvalidOperationExceptionWhenUrlExists()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            var model = StaticMethods.CreateUrlModel();

            repoMock.Setup(r => r.UpdateAsync(It.Is<Url>(u => u.OriginalURL == model.OriginalUrl)))
                .ThrowsAsync(new DbUpdateException("UNIQUE"));

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => service.UpdateAsync(model), "The service must throw InvalidOperationException.");
        }

        [Fact]
        public async Task DeleteAsync_ShuldThrowInvalidOperationExceptionWhenIdDoesntExist()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            int id = 1;

            repoMock.Setup(r => r.DeleteAsync(It.Is<int>(num => num == id)))
                .ThrowsAsync(new KeyNotFoundException());

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => service.DeleteAsync(id), "The service must throw InvalidOperationException.");
        }

        [Fact]
        public async Task GetByIdAsync_ShuldReturnNullWhenIdDoesntExist()
        {
            // Arrange
            var repoMock = new Mock<IUrlRepository>();
            int id = 1;

            repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Url?)null);

            var service = new UrlService(repoMock.Object, urlShorteningService);

            // Act
            var result = await service.GetByIdAsync(id);

            // Assert
            result.ShouldBeNull();
        }
    }
}
