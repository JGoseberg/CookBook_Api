using AutoMapper;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.DTOs;
using CookBook_Api.Enums;
using CookBook_Api.Interfaces.IRecipeService;
using CookBook_Api.Interfaces.IRepositories;
using CookBook_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookBook_Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class RecipesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IRecipeService _recipeService;

        public RecipesController(IMapper mapper, IRecipeRepository recipeRepository, IRecipeService recipeService)
        {
            _mapper = mapper;
            _recipeRepository = recipeRepository;
            _recipeService = recipeService;
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
        public async Task<ActionResult> GetRecipeById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _recipeRepository.GetRecipeByIdAsync(id);

            if (!result.IsSuccess)
            {
                var (status, response) = ErrorHttpMapper.Map(result.Error!);
                return StatusCode(status, response);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipesBySearchString(string searchString, SelectedOperator selectedOperator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _recipeService.SearchRecipesAsync(searchString, selectedOperator);

            if (!result.IsSuccess)
            {
                var (status, response) = ErrorHttpMapper.Map(result.Error!);
                return StatusCode(status, response);
            }

            return Ok(result.Value);
        }
    }
}
