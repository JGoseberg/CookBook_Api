using CookBook_Api.Common;
using CookBook_Api.DTOs;
using CookBook_Api.Models;

namespace CookBook_Api.Interfaces.IServices
{
    public interface IRecipeService
    {
        Result<Recipe?> ValidateAndCreateRecipe(RecipeDTO recipeDTO, UpdateRecipeDTO updateRecipeDTO); 
        Result<Uri?> ValidateAndParseUri (string? uri);

    }
}
