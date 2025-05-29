namespace CookBook_Api.DTOs
{
    public class AddRecipeDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Uri { get; set; }
    }
}
