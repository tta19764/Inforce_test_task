using Microsoft.EntityFrameworkCore;
using Shouldly;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Repositories;

namespace URLShortener.Tests.Repositories
{
    public class URLShortenerUrlRepositoryTests
    {
        [Fact]
        public async Task AddUrl_ShouldAddUrlToDatabase()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var url = StaticMethods.CreateUrl(0, "https://example.com", "exmpl");

            // Act
            var addedUrl = await repository.AddAsync(url);

            // Assert
            addedUrl.ShouldNotBeNull("Added URL is null.");
            addedUrl.OriginalURL.ShouldBe("https://example.com", "Original URL does not match.");
            addedUrl.ShortenedURL.ShouldBe("exmpl", "Shortened URL does not match.");
        }

        [Fact]
        public async Task GetUrlById_ShouldReturnCorrectUrl()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var url = StaticMethods.CreateUrl(0, "https://example.com", "exmpl");
            url = await repository.AddAsync(url);

            // Act
            var retrievedUrl = await repository.GetByIdAsync(url.Id);

            // Assert
            retrievedUrl.ShouldNotBeNull("Retrieved URL is null.");
            retrievedUrl!.OriginalURL.ShouldBe("https://example.com", "Original URL does not match.");
            retrievedUrl.ShortenedURL.ShouldBe("exmpl", "Shortened URL does not match.");
        }

        [Fact]
        public async Task GetUrlByIdWithNegativeId_ShouldThrowAnException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(async () => await repository.GetByIdAsync(-1));
        }

        [Fact]
        public async Task DeleteUrl_ShouldRemoveUrlFromDatabase()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var url = StaticMethods.CreateUrl(0, "https://example.com", "exmpl");
            url = await repository.AddAsync(url);

            // Act
            await repository.DeleteAsync(url.Id);

            // Assert
            var deletedUrl = await context.FindAsync<Url>(url.Id);
            deletedUrl.ShouldBeNull("URL was not deleted.");
        }

        [Fact]
        public async Task DeleteUrlWithNegativeId_ShouldThrowAnException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(async () => await repository.DeleteAsync(-1));
        }

        [Fact]
        public async Task UpdateUrl_ShouldModifyUrlInDatabase()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var url = StaticMethods.CreateUrl(0, "https://example.com", "exmpl");
            url = await repository.AddAsync(url);

            // Act
            url.OriginalURL = "https://updated.com";
            var updatedUrl = await repository.UpdateAsync(url);

            // Assert
            updatedUrl.ShouldNotBeNull("Retrieved URL is null.");
            updatedUrl!.OriginalURL.ShouldBe("https://updated.com", "Original URL was not updated.");
        }

        [Fact]
        public async Task UpdateUrlWithNotExistingId_ShouldThrowException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var url = StaticMethods.CreateUrl(0, "https://example.com", "exmpl");
            url = await repository.AddAsync(url);
            url.Id = 2;

            // Assert & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () => await repository.UpdateAsync(url));
        }

        [Fact]
        public async Task GetAllUrls_ShouldReturnAllUrls()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var user = StaticMethods.CreateUser();
            var url1 = StaticMethods.CreateUrl(0, "https://example1.com", "exmpl1", user);
            var url2 = StaticMethods.CreateUrl(0, "https://example2.com", "exmpl2", user);
            await repository.AddAsync(url1);
            await repository.AddAsync(url2);

            // Act
            var allUrls = await repository.GetAllAsync();

            // Assert
            allUrls.ShouldNotBeNull("Retrieved URLs are null.");
            allUrls.Count().ShouldBe(2, "Number of URLs retrieved does not match.");
        }

        [Fact]
        public async Task GetPaginatedUrls_ShouldReturnCorrectPage()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var user = StaticMethods.CreateUser();
            for (int i = 1; i <= 10; i++)
            {
                var url = StaticMethods.CreateUrl(0, $"https://example{i}.com", $"exmpl{i}", user);
                await repository.AddAsync(url);
            }

            // Act
            var paginatedUrls = await repository.GetPaginatedAsync(2, 3);

            // Assert
            paginatedUrls.ShouldNotBeNull("Retrieved URLs are null.");
            paginatedUrls.Count().ShouldBe(3, "Number of URLs retrieved does not match.");
            paginatedUrls.First().OriginalURL.ShouldBe("https://example4.com", "First URL on the page does not match.");
        }

        [Fact]
        public async Task GetPaginatedUrlsWithInvalidPageNumber_ShouldThrowAnException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(async () => await repository.GetPaginatedAsync(0, 5));
        }
    }
}
