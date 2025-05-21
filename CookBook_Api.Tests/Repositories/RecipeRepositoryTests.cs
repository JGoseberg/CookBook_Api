using AutoMapper;
using CookBook_Api.Data;
using CookBook_Api.DTOs;
using CookBook_Api.Mappings;
using CookBook_Api.Models;
using CookBook_Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task GetAllRecipesAsync_ShouldReturnAllRecipes()
        {
            await using var context = new CookBookContext(_contextOptions);

            await context.Recipes.AddRangeAsync(_recipes);
            await context.SaveChangesAsync();

            var repository = new RecipeRepository(context, _mapper);

            var recipes = await repository.GetAllRecipesAsync();

            Assert.That(recipes.Select(r => r.Name), Is.EquivalentTo(_recipes.Select(r => r.Name)));
        }
    }
}
