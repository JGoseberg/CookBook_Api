using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Enums;
using CookBook_Api.Interfaces.IRecipeService;
using CookBook_Api.Interfaces.IRepositories;
using CookBook_Api.Models;

namespace CookBook_Api.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<Result<List<Recipe>>> SearchRecipesAsync(string searchTerm, SelectedOperator selectedOperator)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Result<List<Recipe>>.Fail(ErrorMessages.SearchTearmInvalid);
            }

            Result<List<Recipe>> recipes = null!;

            recipes = await _recipeRepository.SearchRecipesAsync(searchTerm, selectedOperator);

            return recipes;
        }
    }
}
