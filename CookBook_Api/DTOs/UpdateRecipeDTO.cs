namespace CookBook_Api.DTOs
{
    public class UpdateRecipeDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Uri { get; set; }
    }
}
