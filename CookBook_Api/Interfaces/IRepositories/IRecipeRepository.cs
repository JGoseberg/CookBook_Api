using CookBook_Api.Common;
using CookBook_Api.DTOs;
using CookBook_Api.Models;

namespace CookBook_Api.Interfaces.IRepositories
{
    public interface IRecipeRepository
    {
        Task AddRecipeAsync(Recipe recipeDTO);
        Task<IEnumerable<RecipeDTO>> GetAllRecipesAsync();
        Task<Result<RecipeDTO>> GetRecipeByIdAsync(int id);
    }
}
