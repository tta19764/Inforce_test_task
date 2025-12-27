using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Repositories;

namespace URLShortener.Tests.Repositories
{
    public class URLShortenerApoutPageRepositoryTests
    {
        [Fact]
        public async Task UpdateAboutPage_ShouldModifyPageInDatabase()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new AboutPageRepository(context);
            var user = StaticMethods.CreateUser(id: 99);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            var existing = context.AboutPages.First();
            existing.LastModified = DateTime.Now;
            existing.Content = "Test";
            existing.LastModifiedBy = user;

            // Act
            var updatedPage = await repository.UpdateAsync(existing);

            // Assert
            updatedPage.ShouldNotBeNull("Retrieved Page is null.");
            updatedPage.Content.ShouldBe("Test", "Content was not updted.");
        }

        [Fact]
        public async Task GetAboutPageInfo_ShouldReturnPage()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new AboutPageRepository(context);

            // Act
            var page = await repository.GetAboutPageInfo();

            // Assert
            page.ShouldNotBeNull("Retrieved Page is null.");
            page.Content.ShouldNotBeNullOrEmpty("Content was not updted.");
        }
    }
}
