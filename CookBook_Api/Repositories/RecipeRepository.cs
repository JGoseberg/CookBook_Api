using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Data;
using CookBook_Api.DTOs;
using CookBook_Api.Enums;
using CookBook_Api.Interfaces.IRepositories;
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


        public async Task AddRecipeAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<RecipeDTO>> GetAllRecipesAsync()
        {
            var recipes = await _context.Recipes.ToListAsync();

            return _mapper.Map<IEnumerable<RecipeDTO>>(recipes);
        }


        public async Task<Result<RecipeDTO>> GetRecipeByIdAsync(int id)
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(x => x.Id == id);

            if (recipe == null)
                return Result<RecipeDTO>.Fail(ErrorMessages.RecipeNotFound);

            return Result<RecipeDTO>.Success(_mapper.Map<RecipeDTO>(recipe));
        }

        public async Task<Result<List<Recipe>>> SearchRecipesAsync(string searchTerm, SelectedOperator selectedOperator)
        {
            IQueryable<Recipe> query = _context.Recipes;

            if (selectedOperator == SelectedOperator.Equals)
            {
                query = query.Where(r => r.Name.Equals(searchTerm));
            }
            else
            {
                query = query.Where(r => r.Name.Contains(searchTerm));
            }

            var recipes = await query.ToListAsync();

            if (recipes.Count == 0)
            {
                return Result<List<Recipe>>.Fail(ErrorMessages.RecipeNotFound);
            }

            return Result<List<Recipe>>.Success(recipes);
        }
    }
}
