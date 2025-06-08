using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.DTOs;
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

        [Test]
        public void ValidateAndCreateRecipe_UriNotChanged_ShouldReturnSuccess()
        {
            var existingUri = new Uri("https://example.com");

            var recipeDTO = new RecipeDTO
            {
                Id = 1,
                Name = "Existing Recipe",
                Description = "Existing description",
                Uri = existingUri
            };

            var updateRecipeDTO = new UpdateRecipeDTO
            {
                Id = 1,
                Name = "Updated Recipe",
                Description = "Updated description",
                Uri = existingUri.ToString()
            };

            var result = _recipeService.ValidateAndCreateRecipe(recipeDTO, updateRecipeDTO);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Warnings, Is.Empty);
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value!.Uri, Is.EqualTo(existingUri));
            });
        }

        [Test]
        public void ValidateAndCreateRecipe_ShouldReturnSuccessWithWarnings()
        {
            var recipeDTO = new RecipeDTO
            {
                Id = 1,
                Name = "Existing Recipe",
                Description = "Existing description",
                Uri = new Uri("https://example.com")
            };

            var updateRecipeDTO = new UpdateRecipeDTO
            {
                Id = 1,
                Name = "Updated Recipe",
                Description = "Updated description",
                Uri = "invalid"
            };

            var result = _recipeService.ValidateAndCreateRecipe(recipeDTO, updateRecipeDTO);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Warnings, Is.Not.Empty);
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value!.Uri, Is.Null);
            });
        }

        [Test]
        public void ValidateAndCreateRecipe_UriChanged_ShouldReturnSuccess()
        {
            var recipeDTO = new RecipeDTO
            {
                Id = 1,
                Name = "Existing Recipe",
                Description = "Existing description",
                Uri = new Uri("https://example.com")
            };

            var updateRecipeDTO = new UpdateRecipeDTO
            {
                Id = 1,
                Name = "Updated Recipe",
                Description = "Updated description",
                Uri = "https://newexample.com"
            };

            var result = _recipeService.ValidateAndCreateRecipe(recipeDTO, updateRecipeDTO);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Warnings, Is.Empty);
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value!.Uri, Is.EqualTo(new Uri(updateRecipeDTO.Uri)));
            });
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