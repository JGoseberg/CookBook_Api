using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces.IServices;
using CookBook_Api.Models;

namespace CookBook_Api.Services
{
    public class RecipeService : IRecipeService
    {
        public Result<Recipe?> ValidateAndCreateRecipe(RecipeDTO recipeDTO, UpdateRecipeDTO updateRecipeDTO)
        {
            var newRecipe = new Recipe
            {
                Id = recipeDTO.Id,
                Name = updateRecipeDTO.Name,
                Description = updateRecipeDTO.Description,
            };

            if (recipeDTO.Uri?.ToString() == updateRecipeDTO.Uri)
            {
                newRecipe.Uri = recipeDTO.Uri;
                return Result<Recipe?>.Success(newRecipe);

            }

            var uriResult = ValidateAndParseUri(updateRecipeDTO.Uri);

            List<Error> warnings = [];

            if (!uriResult.IsSuccess)
            {
                warnings.Add(uriResult.Error!);
                return Result<Recipe?>.SuccessWithWarnings(newRecipe, warnings);
            }

            newRecipe.Uri = uriResult.Value;

            return Result<Recipe?>.Success(newRecipe);
        }

        public Result<Uri?> ValidateAndParseUri(string? uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return Result<Uri?>.Success(null);
            }
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var recipeUri) ||
            (recipeUri.Scheme != Uri.UriSchemeHttp && recipeUri.Scheme != Uri.UriSchemeHttps))
            {
                return Result<Uri?>.Fail(ErrorMessages.InvalidUri);
            }
            return Result<Uri?>.Success(recipeUri);
        }
    }
}
