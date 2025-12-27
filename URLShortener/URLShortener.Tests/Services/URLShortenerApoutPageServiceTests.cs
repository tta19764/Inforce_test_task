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

namespace URLShortener.Tests.Services
{
    public class URLShortenerApoutPageServiceTests
    {
        private readonly Mock<IAboutPageRepository> aboutRepoMock = new();
        private readonly Mock<IUserRepository> userRepoMock = new();

        private AboutPageService CreateService()
            => new(aboutRepoMock.Object, userRepoMock.Object);

        [Fact]
        public async Task GetAboutPageInfo_ShouldReturnPage()
        {
            // Arrange
            var page = new AboutPage
            {
                Id = 1,
                Content = "About content",
                CreatedDate = DateTime.UtcNow
            };

            aboutRepoMock
                .Setup(r => r.GetAboutPageInfo())
                .ReturnsAsync(page);

            var service = CreateService();

            // Act
            var result = await service.GetAboutPageInfoAsync();

            // Assert
            result.Content.ShouldBe("About content");
            aboutRepoMock.Verify(r => r.GetAboutPageInfo(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_AdminUser_ShouldUpdatePage()
        {
            // Arrange
            var page = new AboutPage { Id = 1, Content = "Old" };
            var admin = new User
            {
                Id = 1,
                AccountType = new AccountType { TypeName = "Admin" }
            };

            aboutRepoMock.Setup(r => r.GetAboutPageInfo())
                .ReturnsAsync(page);

            aboutRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AboutPage>()))
                .ReturnsAsync(page);

            userRepoMock.Setup(r => r.GetByIdAsync(admin.Id))
                .ReturnsAsync(admin);

            var service = CreateService();

            // Act
            var result = await service.UpdateAsync("New content", admin.Id);

            // Assert
            result.Content.ShouldBe("New content");
            aboutRepoMock.Verify(r => r.UpdateAsync(It.IsAny<AboutPage>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NonAdminUser_ShouldThrow()
        {
            // Arrange
            var user = new User
            {
                Id = 2,
                AccountType = new AccountType { TypeName = "Regular" }
            };

            aboutRepoMock.Setup(r => r.GetAboutPageInfo())
                .ReturnsAsync(new AboutPage());

            userRepoMock.Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            var service = CreateService();

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(
                () => service.UpdateAsync("New content", user.Id));
        }

        [Fact]
        public async Task UpdateAsync_UserNotFound_ShouldThrow()
        {
            // Arrange
            userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User?)null);

            aboutRepoMock.Setup(r => r.GetAboutPageInfo())
                .ReturnsAsync(new AboutPage());

            var service = CreateService();

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(
                () => service.UpdateAsync("Content", 999));
        }
    }
}
