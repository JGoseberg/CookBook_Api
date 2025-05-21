using CookBook_Api.Models;

namespace CookBook_Api.Data
{
    public class DbInitializer
    {
        public static void Initialize(CookBookContext context)
        {
            context.Database.EnsureCreated();

            // Return because Database is seeded
            if (context.Recipes.Any())
                return;

            var recipes = new Recipe[]
            {
                new Recipe{Name="Schinkennudeln"},
                new Recipe{Name="Schweinemedaillons"},
            };

            context.Recipes.AddRange(recipes);
            context.SaveChanges();
        }
    }
}
