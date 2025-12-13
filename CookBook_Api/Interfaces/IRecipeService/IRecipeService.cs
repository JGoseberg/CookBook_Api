using CookBook_Api.Common;
using CookBook_Api.Enums;
using CookBook_Api.Models;

namespace CookBook_Api.Interfaces.IRecipeService
{
    public interface IRecipeService
    {
        Task<Result<List<Recipe>>> SearchRecipesAsync(string searchTearm, SelectedOperator selectedOperator);
    }
}
