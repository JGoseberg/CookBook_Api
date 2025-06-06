using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Services;

namespace CookBook_Api.Tests.Services
{
    [TestFixture]
    internal class RecipeServiceTests()
    {
        private RecipeService _recipeService;

        [SetUp]
        public void Setup()
        {
            _recipeService = new RecipeService();
        }

        [TestCase("http://example.com")]
        [TestCase("https://example.com")]
        public void ValidateAndParseUri_ShouldReturnSuccessAndUri(string uri)
        {
            var result = _recipeService.ValidateAndParseUri(uri);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);

                Assert.That(result.Value, Is.EqualTo(new Uri(uri)));
            });
        }

        [TestCase("ftp://example.com")]
        [TestCase("example.com")]
        public void ValidateAndParseUri_ShouldReturnFailAndErrorMessage(string uri)
        {
            var result = _recipeService.ValidateAndParseUri(uri);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);

                Assert.That(result.Error?.Code, Is.EqualTo(ErrorMessages.InvalidUri.Code));
                Assert.That(result.Error?.Message, Is.EqualTo(ErrorMessages.InvalidUri.Message));
            });
        }

        [TestCase("")]
        [TestCase("   ")]
        public void ValidateAndParseUri_EmptyOrWhitespace_ShouldReturnSuccessWithNull(string uri)
        {
            var result = _recipeService.ValidateAndParseUri(uri);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Value, Is.EqualTo(null));
            });
        }
    }
}