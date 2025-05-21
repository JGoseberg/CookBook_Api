using CookBook_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBook_Api.Data
{
    public class CookBookContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
    
        public CookBookContext(DbContextOptions<CookBookContext> options)
            : base(options) 
        {   }
    }
}
