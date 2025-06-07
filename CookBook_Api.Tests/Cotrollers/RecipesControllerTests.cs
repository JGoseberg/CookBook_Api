using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Controllers;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces.IRepositories;
using CookBook_Api.Mappings;
using CookBook_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CookBook_Api.Tests.Cotrollers
{
    [TestFixture]
    public class RecipesControllerTests
    {
        private Mock<IRecipeRepository> _recipeRepositoryMock;
        private IMapper _mapper;

        private RecipesController _controller;

        [SetUp]
        public void Setup()
        {
            _recipeRepositoryMock = new Mock<IRecipeRepository>();
            _mapper = MapperConfig.InitializeAutoMapper();
            _controller = new RecipesController(_recipeRepositoryMock.Object, _mapper);
        }

        [TestCase("http://example.com")]
        [TestCase("https://example.com")]
        public async Task AddRecipeShouldReturnCreated(string uri)
        {
            var recipeToAdd = new AddRecipeDTO { Name = "Foo", Description = "Bar", Uri = uri };

            var expectedRecipe = new RecipeDTO { Id = 1, Name = "Foo", Description = "Bar", Uri = new Uri(uri) };

            _recipeRepositoryMock.Setup(r => r.AddRecipeAsync(recipeToAdd))
                .ReturnsAsync(Result<RecipeDTO>.Success(expectedRecipe));

            var result = await _controller.AddRecipe(recipeToAdd);
            var createdResult = result as CreatedResult;
            var recipeDTO = createdResult?.Value as RecipeDTO;

            Assert.Multiple(() =>
            {
                Assert.That(createdResult, Is.Not.Null);
                Assert.That(createdResult?.StatusCode, Is.EqualTo(201));

                Assert.That(recipeDTO, Is.Not.Null);
                Assert.That(recipeDTO?.Id, Is.EqualTo(expectedRecipe.Id));
                Assert.That(recipeDTO?.Name, Is.EqualTo(expectedRecipe.Name));
                Assert.That(recipeDTO?.Description, Is.EqualTo(expectedRecipe.Description));
                Assert.That(recipeDTO?.Uri, Is.EqualTo(expectedRecipe.Uri));
            });
        }

        [TestCase("foobar")]
        [TestCase("ftp://example.com")]
        public async Task AddRecipeWithWrongUriShouldReturnBadRequest(string uri)
        {
            var recipeToAdd = new AddRecipeDTO { Name = "Foo", Description = "Bar", Uri = uri };

            _recipeRepositoryMock.Setup(r => r.AddRecipeAsync(recipeToAdd))
                .ReturnsAsync(Result<RecipeDTO>.Fail(ErrorMessages.InvalidUri));

            var result = await _controller.AddRecipe(recipeToAdd);

            var resultObject = result as BadRequestObjectResult;
            var errorResponse = resultObject?.Value as ErrorResponse;

            Assert.Multiple(() =>
            {
                Assert.That(resultObject?.StatusCode, Is.EqualTo(400));

                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse?.Code, Is.EqualTo(ErrorMessages.InvalidUri.Code));
                Assert.That(errorResponse?.Message, Is.EqualTo(ErrorMessages.InvalidUri.Message));
            });
        }

        [Test]
        public async Task DeleteRecipe_ShouldReturnNoContent()
        {
            _recipeRepositoryMock.Setup(r => r.DeleteRecipeAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await _controller.DeleteRecipe(1);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteRecipe_ShouldReturnNotFound()
        {
            _recipeRepositoryMock.Setup(r => r.DeleteRecipeAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<bool>.Fail(ErrorMessages.RecipeNotFound));

            var result = await _controller.DeleteRecipe(1);

            var resultObject = result as NotFoundObjectResult;

            var resultValue = resultObject?.Value as ErrorResponse;

            Assert.Multiple(() =>
            {
                Assert.That(resultObject?.StatusCode, Is.EqualTo(404));

                Assert.That(resultValue?.Code, Is.EqualTo(ErrorMessages.RecipeNotFound.Code));
                Assert.That(resultValue?.Message, Is.EqualTo(ErrorMessages.RecipeNotFound.Message));
            });
        }

        [Test]
        public async Task GetAllRecipesShouldReturnOk()
        {
            var recipes = new List<RecipeDTO>
            {
                new() { Name = "Foo", Description="Bar", Uri=new Uri("http://foobar.com")},
                new() { Name = "Bar"}
            };

            _recipeRepositoryMock.Setup(r => r.GetAllRecipesAsync())
                .ReturnsAsync(recipes);

            var result = await _controller.GetAllRecipes();

            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

                var recipesFromController = (OkObjectResult?)result.Result;
                var returnedRecipes = recipesFromController?.Value as IEnumerable<Recipe>;

                Assert.That(returnedRecipes?.Count(), Is.EqualTo(2));

                var specificRecipe = returnedRecipes?.FirstOrDefault();

                Assert.That(specificRecipe?.Name, Is.EqualTo(recipes[0].Name));
                Assert.That(specificRecipe?.Description, Is.EqualTo(recipes[0].Description));
                Assert.That(specificRecipe?.Uri, Is.EqualTo(recipes[0].Uri));
            });
        }

        [Test]
        public async Task GetRecipeById_ShouldReturnOk()
        {
            var recipe = new RecipeDTO { Id = 1, Name = "Foo", Description = "Bar", Uri = new Uri("http://foobar.com") };

            _recipeRepositoryMock.Setup(r => r.GetRecipeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<RecipeDTO>.Success(recipe));

            var result = await _controller.GetRecipeById(recipe.Id);

            var resultValue = result.Result as OkObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.Not.Null);

                Assert.That(resultValue?.Value, Is.EqualTo(recipe));
            });
        }

        [Test]
        public async Task GetRecipeById_ShouldReturnNotFound()
        {
            _recipeRepositoryMock.Setup(r => r.GetRecipeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<RecipeDTO>.Fail(ErrorMessages.RecipeNotFound));

            var result = await _controller.GetRecipeById(It.IsAny<int>());

            var resultObject = result.Result as NotFoundObjectResult;

            var error = resultObject?.Value as ErrorResponse;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(resultObject?.StatusCode, Is.EqualTo(404));
                Assert.That(error?.Code, Is.EqualTo(ErrorMessages.RecipeNotFound.Code));
                Assert.That(error?.Message, Is.EqualTo(ErrorMessages.RecipeNotFound.Message));
            });
        }
    }
}
