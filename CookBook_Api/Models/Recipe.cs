namespace CookBook_Api.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Uri? Uri { get; set; }
    }
}
