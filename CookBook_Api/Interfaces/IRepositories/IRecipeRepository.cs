using CookBook_Api.Common;
using CookBook_Api.DTOs;
using CookBook_Api.Models;

namespace CookBook_Api.Interfaces.IRepositories
{
    public interface IRecipeRepository
    {
        Task<Result<RecipeDTO>> AddRecipeAsync(AddRecipeDTO addRecipeDTO);
        Task<Result<bool>> DeleteRecipeAsync(int id);
        Task<IEnumerable<RecipeDTO>> GetAllRecipesAsync();
        Task<Result<RecipeDTO>> GetRecipeByIdAsync(int id);
    }
}
