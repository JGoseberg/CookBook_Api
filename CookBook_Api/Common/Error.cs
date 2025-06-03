using CookBook_Api.Models;

namespace CookBook_Api.Common
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static Error RecipeNotFound = new("RECIPE_NOT_FOUND", "Recipe could not be found.");
    }
}
