using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Data;
using CookBook_Api.Mappings;
using CookBook_Api.Models;
using CookBook_Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CookBook_Api.Tests.Repositories
{
    [TestFixture]
    internal class RecipeRepositoryTests
    {
        private DbContextOptions<CookBookContext> _contextOptions;
        private IMapper _mapper;

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
        }


        [TearDown]
        public void Teardown()
        {
            using var context = new CookBookContext(_contextOptions);
            context.Database.EnsureDeleted();
        }


        [Test]
        public async Task AddRecipeAsync_ShouldBeAddedInDatabase()
        {
            var recipeToAdd = new Recipe { Name = "Foo", Description = "Bar", Uri = new Uri("http://foobar.com") };

            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper);

            await repository.AddRecipeAsync(recipeToAdd);

            var result = await context.Recipes.FirstOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(1));
                Assert.That(result, Is.EqualTo(recipeToAdd));
            });
        }


        [Test]
        public async Task GetAllRecipesAsync_ShouldReturnAllRecipes()
        {
            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper);

            await context.Recipes.AddRangeAsync(_recipes);
            await context.SaveChangesAsync();

            var recipes = await repository.GetAllRecipesAsync();

            Assert.That(recipes.Select(r => r.Name), Is.EquivalentTo(_recipes.Select(r => r.Name)));
        }


        [Test]
        public async Task GetRecipeByIdAsync_ShouldReturnRecipe()
        {
            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper);

            var recipe = new Recipe { Id=1, Name = "Foo", Description = "Bar", Uri = new Uri("http://foobar.com") };

            await repository.AddRecipeAsync(recipe);
            await context.SaveChangesAsync();

            var result = await repository.GetRecipeByIdAsync(recipe.Id);

            Assert.Multiple(() =>
            {
                Assert.That(result!.Value!.Name, Is.EqualTo(recipe.Name));
                Assert.That(result!.Value!.Description, Is.EqualTo(recipe.Description));
                Assert.That(result!.Value!.Uri, Is.EqualTo(recipe.Uri));
            });
        }

        [Test]
        public async Task GetRecipeByIdAsync_ShouldReturnNotFound()
        {
            await using var context = new CookBookContext(_contextOptions);

            var repository = new RecipeRepository(context, _mapper);

            var recipe = new Recipe { Id = 1, Name = "Foo", Description = "Bar", Uri = new Uri("http://foobar.com") };

            await repository.AddRecipeAsync(recipe);
            await context.SaveChangesAsync();

            var result = await repository.GetRecipeByIdAsync(404);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Error, Is.EqualTo(Error.RecipeNotFound));
            });
        }
    }
}
