using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces.IServices;
using CookBook_Api.Models;

namespace CookBook_Api.Services
{
    public class RecipeService : IRecipeService
    {
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
