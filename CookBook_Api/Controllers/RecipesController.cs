using AutoMapper;
using CookBook_Api.Interfaces;
using CookBook_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookBook_Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RecipesController(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            var recipesDtos = await _recipeRepository.GetAllRecipesAsync();

            var recipes = _mapper.Map<IEnumerable<Recipe>>(recipesDtos);
            return Ok(recipes);
            
        }
    }
}
