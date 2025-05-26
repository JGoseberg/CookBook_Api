namespace CookBook_Api.DTOs
{
    public class RecipeDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Uri? Uri { get; set; }
    }
}
