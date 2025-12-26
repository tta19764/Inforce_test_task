using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Database.Servicies;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Services;

namespace URLShortener.Tests
{
    public class URLShortenerUrlServiceTests
    {
        private readonly Mock<IUrlRepository> urlRepoMock;
        private readonly Mock<IUserRepository> userRepoMock;
        private readonly IUrlShorteningService shorteningService;

        public URLShortenerUrlServiceTests()
        {
            urlRepoMock = new Mock<IUrlRepository>();
            userRepoMock = new Mock<IUserRepository>();
            shorteningService = new UrlShorteningService();
        }

        private UrlService CreateService()
            => new(urlRepoMock.Object, userRepoMock.Object, shorteningService);


        [Fact]
        public async Task AddAsync_ShouldGenerateShortCode_AndCallRepository()
        {
            // Arrange
            var model = StaticMethods.CreateUrlModel();
            var entity = StaticMethods.CreateUrl(
                0,
                model.OriginalUrl,
                shorteningService.GenerateShortCode(model.OriginalUrl));

            urlRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Url>()))
                .ReturnsAsync(entity);

            var service = CreateService();

            // Act
            var result = await service.AddAsync(model);

            // Assert
            result.OriginalUrl.ShouldBe(model.OriginalUrl);
            result.ShortenedUrl.ShouldBe(entity.ShortenedURL);

            urlRepoMock.Verify(r =>
                r.AddAsync(It.Is<Url>(u =>
                    u.OriginalURL == model.OriginalUrl &&
                    u.ShortenedURL == entity.ShortenedURL)),
                Times.Once);
        }

        //[Fact]
        //public async Task UpdateAsync_ShuldGenerateShortCodeCallUpdateAync()
        //{
        //    // Arrange
        //    var repoMock = new Mock<IUrlRepository>();
        //    var model = StaticMethods.CreateUrlModel();
        //    var entity = StaticMethods.CreateUrl(model.OriginalUrl, urlShorteningService.GenerateShortCode(model.OriginalUrl));

        //    repoMock.Setup(r => r.UpdateAsync(It.IsAny<Url>()))
        //        .ReturnsAsync(entity);

        //    var service = new UrlService(repoMock.Object, urlShorteningService);

        //    // Act
        //    var result = await service.UpdateAsync(model);

        //    // Assert
        //    result.OriginalUrl.ShouldBe(model.OriginalUrl);
        //    result.ShortenedUrl.ShouldBe(urlShorteningService.GenerateShortCode(model.OriginalUrl));
        //    result.CreatorNickName.ShouldBe(model.CreatorNickName);
        //    repoMock.Verify(r => r.UpdateAsync(It.Is<Url>(url => url.OriginalURL == entity.OriginalURL)), Times.Once);
        //}

        [Fact]
        public async Task GetByIdAsync_ShouldIncrementClickCount_AndUpdate()
        {
            // Arrange
            var entity = StaticMethods.CreateUrl(1, "https://test.com", "abc123");
            entity.ClickCount = 0;

            urlRepoMock.Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            urlRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Url>()))
                .ReturnsAsync(entity);

            var service = CreateService();

            // Act
            var result = await service.GetByIdAsync(entity.Id);

            // Assert
            result.ShouldNotBeNull();
            entity.ClickCount.ShouldBe(1);

            urlRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Url>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenUserIsOwner()
        {
            // Arrange
            var user = StaticMethods.CreateUser(1);
            var url = StaticMethods.CreateUrl(1, "https://test.com", "abc");
            url.CreatorId = user.Id;

            userRepoMock.Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            urlRepoMock.Setup(r => r.GetByIdAsync(url.Id))
                .ReturnsAsync(url);

            urlRepoMock.Setup(r => r.DeleteAsync(url.Id))
                .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            await service.DeleteAsync(url.Id, user.Id);

            // Assert
            urlRepoMock.Verify(r => r.DeleteAsync(url.Id), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenUrlAlreadyExists()
        {
            // Arrange
            var model = StaticMethods.CreateUrlModel();

            urlRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Url>()))
                .ThrowsAsync(new DbUpdateException("UNIQUE"));

            var service = CreateService();

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() =>
                service.AddAsync(model));
        }

        //[Fact]
        //public async Task UpdateAsync_ShuldThrowInvalidOperationExceptionWhenUrlExists()
        //{
        //    // Arrange
        //    var repoMock = new Mock<IUrlRepository>();
        //    var model = StaticMethods.CreateUrlModel();

        //    repoMock.Setup(r => r.UpdateAsync(It.Is<Url>(u => u.OriginalURL == model.OriginalUrl)))
        //        .ThrowsAsync(new DbUpdateException("UNIQUE"));

        //    var service = new UrlService(repoMock.Object, urlShorteningService);

        //    // Act & Assert
        //    await Should.ThrowAsync<InvalidOperationException>(() => service.UpdateAsync(model), "The service must throw InvalidOperationException.");
        //}

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenUserHasNoRights()
        {
            // Arrange
            var user = StaticMethods.CreateUser();
            user.AccountType = new AccountType(1)
            {
                TypeName = "Regular",
            };
            var url = StaticMethods.CreateUrl(1, "https://test.com", "abc", user);
            url.CreatorId = 999;

            userRepoMock.Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            urlRepoMock.Setup(r => r.GetByIdAsync(url.Id))
                .ReturnsAsync(url);

            var service = CreateService();

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() =>
                service.DeleteAsync(url.Id, user.Id));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            urlRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Url?)null);

            var service = CreateService();

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            result.ShouldBeNull();
        }
    }
}
