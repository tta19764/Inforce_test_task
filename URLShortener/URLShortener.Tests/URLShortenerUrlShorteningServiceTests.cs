using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Services;

namespace URLShortener.Tests
{
    public class URLShortenerUrlShorteningServiceTests
    {
        private readonly IUrlShorteningService urlShorteningService;

        public URLShortenerUrlShorteningServiceTests()
        {
            urlShorteningService = new UrlShorteningService();
        }

        [Fact]
        public void GenerateShortCode_SameUrl_ReturnsSameCode()
        {
            // Arrange
            var url = "https://example.com";

            // Act
            var code1 = urlShorteningService.GenerateShortCode(url);
            var code2 = urlShorteningService.GenerateShortCode(url);

            // Assert
            code1.ShouldBe(code2);
            code1.Length.ShouldBe(8);
        }

        [Fact]
        public void GenerateShortCode_DifferentUrls_ReturnDifferentCodes()
        {
            // Arrange
            var url1 = "https://example.com";
            var url2 = "https://example.com/page";

            // Act
            var code1 = urlShorteningService.GenerateShortCode(url1);
            var code2 = urlShorteningService.GenerateShortCode(url2);

            // Assert
            code1.ShouldNotBe(code2);
            code1.Length.ShouldBe(8);
            code2.Length.ShouldBe(8);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerateShortCode_InvalidUrl_ThrowsArgumentException(string invalidUrl)
        {
            // Act
            var exception = Should.Throw<ArgumentException>(() =>
                urlShorteningService.GenerateShortCode(invalidUrl));

            // Assert
            exception.ShouldNotBeNull();
        }
    }
}
