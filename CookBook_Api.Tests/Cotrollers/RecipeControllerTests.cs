using CookBook_Api.Controllers;
using CookBook_Api.Interfaces;
using CookBook_Api.DTOs;
using AutoMapper;
using CookBook_Api.Mappings;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CookBook_Api.Models;

namespace CookBook_Api.Tests.Cotrollers
{
    [TestFixture]
    public class RecipeControllerTests
    {
        private Mock<IRecipeRepository> _recipeRepositoryMock;
        private IMapper _mapper;

        private RecipeController _controller;

        [SetUp]
        public void Setup()
        {
            _recipeRepositoryMock = new Mock<IRecipeRepository>();
            _mapper = MapperConfig.InitializeAutoMapper();
            _controller = new RecipeController(_recipeRepositoryMock.Object, _mapper);
        }

        [Test]
        public async Task GetAllRecipesShouldReturnOk()
        {
            var recipes = new List<RecipeDTO>
            {
                new() { Name = "Foo"},
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
            });
        }
    }
}
