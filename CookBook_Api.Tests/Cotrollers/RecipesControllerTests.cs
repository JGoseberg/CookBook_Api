using AutoMapper;
using CookBook_Api.Common;
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

        [Test]
        public async Task AddRecipeShouldReturnCreated()
        {
            var recipeToAdd = new AddRecipeDTO { Name = "Foo", Description = "Bar", Uri = "http://foobar.com" };

            var result = await _controller.AddRecipe(recipeToAdd);

            Assert.Multiple(() =>
            {
                var okResult = result as CreatedResult;

                Assert.That(okResult, Is.Not.Null);

                var recipeResult = okResult?.Value as Recipe;

                Assert.That(recipeResult?.Name, Is.EqualTo(recipeToAdd.Name));
                Assert.That(recipeResult?.Description, Is.EqualTo(recipeToAdd.Description));
                Assert.That(recipeResult?.Uri, Is.TypeOf<Uri>());
            });
        }

        [Test]
        public async Task AddRecipeWithWrongUriShouldReturnCreated()
        {
            var recipeToAdd = new AddRecipeDTO { Name = "Foo", Description = "Bar", Uri = "foobar" };

            var result = await _controller.AddRecipe(recipeToAdd);

            Assert.Multiple(() =>
            {
                var okResult = result as CreatedResult;

                Assert.That(okResult, Is.Not.Null);

                var recipeResult = okResult?.Value as Recipe;

                Assert.That(recipeResult?.Name, Is.EqualTo(recipeToAdd.Name));
                Assert.That(recipeResult?.Description, Is.EqualTo(recipeToAdd.Description));
                Assert.That(recipeResult?.Uri, Is.Null);
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
                .ReturnsAsync(Result<RecipeDTO>.Fail(Error.RecipeNotFound));

            var result = await _controller.GetRecipeById(It.IsAny<int>());

            var resultObject = result.Result as NotFoundObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(resultObject?.StatusCode, Is.EqualTo(404));
                Assert.That(resultObject?.Value, Is.EqualTo("Recipe could not be found"));
            });
        }
    }
}
