using AutoMapper;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces;
using CookBook_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookBook_Api.Controllers
{
    [ApiController]
    [Route("/api/")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RecipeController(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            var recipes = await _recipeRepository.GetAllRecipesAsync();

            var test = _mapper.Map<IEnumerable<Recipe>>(recipes);
            return Ok(test);
            
        }
    }
}
