namespace CookBook_Api.Common.ErrorHandling
{
    public static class ErrorMessages
    {
        public static readonly Error RecipeNotFound = new("RECIPE_NOT_FOUND", "Recipe could not be found.");
        public static readonly Error InvalidUri = new("Invalid_URI", "If provided URI must be an absolute HTTP or HTTPS URL");

    }
}
