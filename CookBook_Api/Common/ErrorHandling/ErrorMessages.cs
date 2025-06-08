namespace CookBook_Api.Common.ErrorHandling
{
    public static class ErrorMessages
    {
        public static readonly Error RecipeNotFound = new("RECIPE_NOT_FOUND", "Recipe could not be found.");
        public static readonly Error InvalidUri = new("INVALID_URI", "If provided URI must be an absolute HTTP or HTTPS URL");
        public static readonly Error IdsNotMatching = new("IDS_NOT_MATCHING", "Id must match the Id with Provided Recipe");
    }
}
