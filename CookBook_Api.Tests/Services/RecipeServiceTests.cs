using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Enums;
using CookBook_Api.Interfaces.IRepositories;
using CookBook_Api.Models;
using CookBook_Api.Services;
using Moq;

namespace CookBook_Api.Tests.Services
{
    [TestFixture]
    public class RecipeServiceTests
    {
        private Mock<IRecipeRepository> _recipeRepositoryMock;
        private RecipeService _recipeService;

        [SetUp]
        public void Setup()
        {
            _recipeRepositoryMock = new Mock<IRecipeRepository>();
            _recipeService = new RecipeService(_recipeRepositoryMock.Object);

        }

        [Test]
        public async Task SerchRecipesAsync_WithEmptySearchTerm_ReturnsError()
        {
            var result = await _recipeService.SearchRecipesAsync("", SelectedOperator.Equals);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error, Is.EqualTo(ErrorMessages.SearchTearmInvalid));
            });

            _recipeRepositoryMock.Verify(
                r => r.SearchRecipesAsync(It.IsAny<string>(), It.IsAny<SelectedOperator>()),
                Times.Never);
        }

        [Test]
        public async Task SerchRecipesAsync_RepositoryReturnsNotFound_ReturnsNotFound()
        {
            string searchTerm = "foo";

            _recipeRepositoryMock
                .Setup(r => r.SearchRecipesAsync(searchTerm, SelectedOperator.Equals))
                .ReturnsAsync(Result<List<Recipe>>.Fail(ErrorMessages.RecipeNotFound));

            var result = await _recipeService.SearchRecipesAsync(searchTerm, SelectedOperator.Equals);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error, Is.EqualTo(ErrorMessages.RecipeNotFound));
            });
        }

        [Test]
        public async Task SerchRecipesAsync_RepositoryReturnsRecipes_ReturnsRecipes()
        {
            string searchTerm = "foo";

            var recipes = new List<Recipe>{ new Recipe() { Name = "Foo" } };

            _recipeRepositoryMock
                .Setup(r => r.SearchRecipesAsync(searchTerm, SelectedOperator.Equals))
                .ReturnsAsync(Result<List<Recipe>>.Success(recipes));

            var result = await _recipeService.SearchRecipesAsync(searchTerm, SelectedOperator.Equals);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Has.Count.EqualTo(1));
                Assert.That(result.Value![0].Name, Is.EqualTo("Foo"));
            });
        }
    }
}
