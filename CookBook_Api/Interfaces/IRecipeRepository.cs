using CookBook_Api.DTOs;
using CookBook_Api.Models;

namespace CookBook_Api.Interfaces
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<RecipeDTO>> GetAllRecipesAsync();
    }
}
