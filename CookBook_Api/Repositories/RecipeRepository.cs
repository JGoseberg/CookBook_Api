using AutoMapper;
using CookBook_Api.Data;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces;
using CookBook_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBook_Api.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly CookBookContext _context;
        private readonly IMapper _mapper;

        public RecipeRepository(CookBookContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipeDTO>> GetAllRecipesAsync()
        {
            var recipes = await _context.Recipes.ToListAsync();

            return _mapper.Map<IEnumerable<RecipeDTO>>(recipes);
        }
    }
}
