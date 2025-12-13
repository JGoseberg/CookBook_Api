using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Controllers;
using CookBook_Api.DTOs;
using CookBook_Api.Enums;
using CookBook_Api.Interfaces.IRecipeService;
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
        private Mock<IRecipeService>    _recipeServiceMock;
        private IMapper _mapper;

        private RecipesController _controller;

        [SetUp]
        public void Setup()
        {
            _recipeServiceMock      = new Mock<IRecipeService>();
            _recipeRepositoryMock   = new Mock<IRecipeRepository>();
            _mapper                 = MapperConfig.InitializeAutoMapper();
            
            _controller             = new RecipesController(_mapper, _recipeRepositoryMock.Object, _recipeServiceMock.Object );
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

            var resultValue = result as ObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                           
                Assert.That(resultValue?.Value, Is.EqualTo(recipe));
            });
        }

        [Test]
        public async Task GetRecipeById_ShouldReturnNotFound()
        {
            _recipeRepositoryMock.Setup(r => r.GetRecipeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<RecipeDTO>.Fail(ErrorMessages.RecipeNotFound));

            var result = await _controller.GetRecipeById(It.IsAny<int>());

            var resultObject = result as ObjectResult;

            var error = resultObject?.Value as ErrorResponse;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(resultObject?.StatusCode, Is.EqualTo(404));
                Assert.That(error?.Code, Is.EqualTo((404).ToString()));
                Assert.That(error?.Message, Is.EqualTo(ErrorMessages.RecipeNotFound.Message));
            });
        }

        [Test]
        public async Task GetRecipeBySearchStringWithContainsShouldReturnNotFound()
        {
            string searchString = "pasta";
            SelectedOperator op = SelectedOperator.Contains;

            _recipeServiceMock
                .Setup(s => s.SearchRecipesAsync(searchString, op))
                .ReturnsAsync(Result<List<Recipe>>.Fail(ErrorMessages.RecipeNotFound));

            var result = await _controller.GetRecipesBySearchString(searchString, op);

            var statusResult = result.Result as ObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(statusResult, Is.Not.Null);
                Assert.That(statusResult?.StatusCode, Is.EqualTo(404));
            });
        }

        [Test]
        public async Task GetRecipeBySearchStringWithContainsShouldReturnListOfRecipes()
        {
            string searchString = "sin";
            SelectedOperator op = SelectedOperator.Contains;

            var recipe = new Recipe { Id = 1, Name = "Chilli sin carne", Description = "Bar", Uri = new Uri("http://foobar.com") };
            var recipe2 = new Recipe { Id = 2, Name = "Chilli con carne", Description = "Bar", Uri = new Uri("http://foobar.com") };

            _recipeServiceMock
                .Setup(s => s.SearchRecipesAsync(searchString, op))
                .ReturnsAsync(Result<List<Recipe>>.Success(new List<Recipe> { recipe }));

            var result = await _controller.GetRecipesBySearchString(searchString, op);

            var statusResult = result.Result as ObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(statusResult, Is.Not.Null);
                Assert.That(statusResult?.StatusCode, Is.EqualTo(200));
                Assert.That(statusResult?.Value, Is.EqualTo(new List<Recipe> { recipe }));
            });
        }

        [Test]
        public async Task GetRecipeBySearchStringWithEqualssShouldReturnNotFound()
        {
            string searchString = "pasta";
            SelectedOperator op = SelectedOperator.Equals;

            _recipeServiceMock
                .Setup(s => s.SearchRecipesAsync(searchString, op))
                .ReturnsAsync(Result<List<Recipe>>.Fail(ErrorMessages.RecipeNotFound));

            var result = await _controller.GetRecipesBySearchString(searchString, op);

            var statusResult = result.Result as ObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(statusResult, Is.Not.Null);
                Assert.That(statusResult?.StatusCode, Is.EqualTo(404));
            });
        }

        [Test]
        public async Task GetRecipeBySearchStringEqualsShouldReturnListOfRecipes()
        {
            string searchString = "sin carne";
            SelectedOperator op = SelectedOperator.Equals;

            var recipe = new Recipe { Id = 1, Name = "sin carne", Description = "Bar", Uri = new Uri("http://foobar.com") };
            var recipe2 = new Recipe { Id = 2, Name = "Chilli sin carne", Description = "Bar", Uri = new Uri("http://foobar.com") };

            _recipeServiceMock
                .Setup(s => s.SearchRecipesAsync(searchString, op))
                .ReturnsAsync(Result<List<Recipe>>.Success(new List<Recipe> { recipe }));

            var result = await _controller.GetRecipesBySearchString(searchString, op);

            var statusResult = result.Result as ObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(statusResult, Is.Not.Null);
                Assert.That(statusResult?.StatusCode, Is.EqualTo(200));
                Assert.That(statusResult?.Value, Is.EqualTo(new List<Recipe> { recipe }));
            });
        }
    }
}
