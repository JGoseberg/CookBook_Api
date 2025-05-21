using CookBook_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBook_Api.Data
{
    public class CookBookContext(DbContextOptions<CookBookContext> options) : DbContext(options)
    {
        public DbSet<Recipe> Recipes { get; set; }
    }
}
