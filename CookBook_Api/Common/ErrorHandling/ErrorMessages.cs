using CookBook_Api.Enums;

namespace CookBook_Api.Common.ErrorHandling
{
    public static class ErrorMessages
    {
        public static readonly Error RecipeNotFound         = new(ErrorCode.RecipeNotFound,     "Recipe could not be found.");
        public static readonly Error SearchTearmInvalid     = new(ErrorCode.SearchTermInvalid,  "Search Term could not be empty.");
        public static readonly Error UnexpectedError        = new(ErrorCode.UnexpectedError,    "An Unexpected error occured.");
    }
}
