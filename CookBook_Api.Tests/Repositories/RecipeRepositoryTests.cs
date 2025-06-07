using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Data;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces.IServices;
using CookBook_Api.Mappings;
using CookBook_Api.Models;
using CookBook_Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CookBook_Api.Tests.Repositories
{
    [TestFixture]
    internal class RecipeRepositoryTests
    {
        private DbContextOptions<CookBookContext> _contextOptions;
        private IMapper _mapper;
        private Mock<IRecipeService> _recipeService;

        private readonly IEnumerable<Recipe> _recipes = new Recipe[]
        {
            new Recipe {Name="Foo"},
            new Recipe {Name="Bar"},
        };


        [SetUp]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<CookBookContext>()
                .UseInMemoryDatabase("CookBook")
                .Options;

            _mapper = MapperConfig.InitializeAutoMapper();

            _recipeService = new Mock<IRecipeService>();
        }


        [TearDown]
        public void Teardown()
        {
            using var context = new CookBookContext(_contextOptions);
            context.Database.EnsureDeleted();
        }


        [TestCase("http://example.com")]
        [TestCase("https://example.com")]
        public async Task AddRecipeAsync_ShouldBeAddedInDatabase(string uri)
        {
            var recipeToAdd = new AddRecipeDTO { Name = "Foo", Description = "Bar", Uri = uri };

            _recipeService.Setup(r => r.ValidateAndParseUri(uri))
                .Returns(Result<Uri?>.Success(new Uri(uri)));

            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper, _recipeService.Object);

            await repository.AddRecipeAsync(recipeToAdd);

            var expectedUri = new Uri(uri);
            var result = await context.Recipes.FirstOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(1));
                Assert.That(result?.Name, Is.EqualTo(recipeToAdd.Name));
                Assert.That(result?.Description, Is.EqualTo(recipeToAdd.Description));
                Assert.That(result?.Uri, Is.EqualTo(expectedUri));
            });
        }

        [TestCase("ftp://example.com")]
        [TestCase("example.com")]
        public async Task AddRecipeAsync_UriShouldNotBeAddedInDatabase(string uri)
        {
            var recipeToAdd = new AddRecipeDTO { Name = "Foo", Description = "Bar", Uri = uri };

            _recipeService.Setup(r => r.ValidateAndParseUri(uri))
                .Returns(Result<Uri?>.Fail(ErrorMessages.InvalidUri));

            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper, _recipeService.Object);

            var result = await repository.AddRecipeAsync(recipeToAdd);

            var recipeInDb = await context.Recipes.FirstOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);

                Assert.That(recipeToAdd.Name, Is.EqualTo(result.Value?.Name));
                Assert.That(result.Value?.Name, Is.EqualTo(recipeInDb?.Name));

                Assert.That(recipeToAdd.Description, Is.EqualTo(result.Value?.Description));
                Assert.That(result.Value?.Description, Is.EqualTo(recipeInDb?.Description));

                Assert.That(result.Value?.Uri, Is.Null);
                Assert.That(recipeInDb?.Uri, Is.Null);
            });
        }

        [Test]
        public async Task DeleteRecipe_ShouldRemoveRecipeFromDb()
        {
            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper, _recipeService.Object);

            await context.AddRangeAsync(_recipes);
            await context.SaveChangesAsync();

            var result = await repository.DeleteRecipeAsync(1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);

                Assert.That(context.Recipes.FirstOrDefault()?.Id, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task DeleteRecipe_ShouldReturnNotFoundResult()
        {
            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper, _recipeService.Object);

            await context.AddRangeAsync(_recipes);
            await context.SaveChangesAsync();

            var result = await repository.DeleteRecipeAsync(404);

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsSuccess);

                Assert.That(context.Recipes.FirstOrDefault()?.Id, Is.EqualTo(1));

                Assert.That(result.Error?.Code, Is.EqualTo(ErrorMessages.RecipeNotFound.Code));
                Assert.That(result.Error?.Message, Is.EqualTo(ErrorMessages.RecipeNotFound.Message));
            });
        }

        [Test]
        public async Task GetAllRecipesAsync_ShouldReturnAllRecipes()
        {
            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper, _recipeService.Object);

            await context.Recipes.AddRangeAsync(_recipes);
            await context.SaveChangesAsync();

            var recipes = await repository.GetAllRecipesAsync();

            Assert.That(recipes.Select(r => r.Name), Is.EquivalentTo(_recipes.Select(r => r.Name)));
        }


        [Test]
        public async Task GetRecipeByIdAsync_ShouldReturnRecipe()
        {
            var uri = "http://foobar.com";

            var recipe = new AddRecipeDTO { Name = "Foo", Description = "Bar", Uri = uri };

            _recipeService.Setup(r => r.ValidateAndParseUri(uri))
                .Returns(Result<Uri?>.Success(new Uri(uri)));

            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper, _recipeService.Object);

            await repository.AddRecipeAsync(recipe);
            await context.SaveChangesAsync();

            var expectedUri = new Uri(uri);
            var result = await repository.GetRecipeByIdAsync(1);

            Assert.Multiple(() =>
            {
                Assert.That(result!.Value!.Name, Is.EqualTo(recipe.Name));
                Assert.That(result!.Value!.Description, Is.EqualTo(recipe.Description));
                Assert.That(result!.Value!.Uri, Is.EqualTo(expectedUri));
            });
        }

        [Test]
        public async Task GetRecipeByIdAsync_ShouldReturnNotFound()
        {
            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper, _recipeService.Object);

            var result = await repository.GetRecipeByIdAsync(404);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);

                Assert.That(result.Error?.Code, Is.EqualTo(ErrorMessages.RecipeNotFound.Code));
                Assert.That(result.Error?.Message, Is.EqualTo(ErrorMessages.RecipeNotFound.Message));
            });
        }
    }
}
