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
    public class URLShortenerPasswordHasherTests
    {
        private readonly IPasswordHasher passwordHasher;

        public URLShortenerPasswordHasherTests()
        {
            passwordHasher = new PasswordHasher();
        }

        [Theory]
        [InlineData("TestPassword1", "testpassword1", false)]
        [InlineData("TestPassword2", "TestPassword2", true)]
        [InlineData("Test3", "test3", false)]
        public void PasswordHasherVerify_ReturnsValidResult(string passwordToHash, string passwordToCompare, bool expectedResult)
        {
            // Arrange
            var hasedPassword = passwordHasher.Hash(passwordToHash);

            // Act & Assert
            passwordHasher.Verify(passwordToCompare, hasedPassword).ShouldBe(expectedResult, "The return of the hasher is not an expected result.");
        }
    }
}
