using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.Data;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces.IRepositories;
using CookBook_Api.Interfaces.IServices;
using CookBook_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBook_Api.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly CookBookContext _context;
        private readonly IMapper _mapper;
        private readonly IRecipeService _recipeService;

        public RecipeRepository(CookBookContext context, IMapper mapper, IRecipeService recipeService)
        {
            _context = context;
            _mapper = mapper;
            _recipeService = recipeService;
        }


        public async Task<Result<RecipeDTO>> AddRecipeAsync(AddRecipeDTO addRecipe)
        {
            var recipe = new Recipe
            {
                Name = addRecipe.Name,
                Description = addRecipe.Description,
            };

            List<Error> warnings = [];

            var uri = _recipeService.ValidateAndParseUri(addRecipe.Uri);
            if (uri.IsSuccess)
                recipe.Uri = uri.Value;
            else if (!string.IsNullOrWhiteSpace(addRecipe.Uri))
                warnings.Add(uri.Error!);

                await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            var recipeDTO = _mapper.Map<RecipeDTO>(recipe);

            if (warnings.Count > 0) 
                return Result<RecipeDTO>.SuccessWithWarnings(recipeDTO, warnings);

            return Result<RecipeDTO>.Success(recipeDTO);
        }

        public async Task<Result<bool>> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
                return Result<bool>.Fail(ErrorMessages.RecipeNotFound);

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true);
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
    }
}
