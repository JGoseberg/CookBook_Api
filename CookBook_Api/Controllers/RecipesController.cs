using AutoMapper;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces.IRepositories;
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


        [HttpPost]
        public async Task<ActionResult> AddRecipe([FromBody]AddRecipeDTO addRecipe)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var recipe = new Recipe { Name = addRecipe.Name, Description = addRecipe.Description };
            
            if (Uri.TryCreate(addRecipe.Uri, UriKind.Absolute, out var recipeUri))
                recipe.Uri = recipeUri;

            await _recipeRepository.AddRecipeAsync(recipe);

            return Created("", recipe);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            var recipesDtos = await _recipeRepository.GetAllRecipesAsync();

            var recipes = _mapper.Map<IEnumerable<Recipe>>(recipesDtos);

            return Ok(recipes);
        }

        [HttpGet]
        public async Task<ActionResult<RecipeDTO>> GetRecipeById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var recipe = await _recipeRepository.GetRecipeByIdAsync(id);

            if (!recipe.IsSuccess)
                return NotFound(new {code = recipe.Error!.Code, message = recipe.Error.Message });

            return Ok(recipe.Value);
        }
    }
}
