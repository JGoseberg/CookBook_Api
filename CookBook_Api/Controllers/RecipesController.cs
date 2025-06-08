using AutoMapper;
using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using CookBook_Api.DTOs;
using CookBook_Api.Interfaces.IRepositories;
using CookBook_Api.Interfaces.IServices;
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
        public async Task<ActionResult> AddRecipe([FromBody] AddRecipeDTO addRecipe)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _recipeRepository.AddRecipeAsync(addRecipe);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.CreateFromError(result.Error!));

            return Created("", result.Value);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _recipeRepository.DeleteRecipeAsync(id);
            if (!result.IsSuccess)
                return NotFound(ErrorResponse.CreateFromError(result.Error!));

            return NoContent();
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
                return BadRequest(ModelState);

            var recipe = await _recipeRepository.GetRecipeByIdAsync(id);

            if (!recipe.IsSuccess)
                return NotFound(ErrorResponse.CreateFromError(recipe.Error!));

            return Ok(recipe.Value);
        }

        [HttpPatch]
        public async Task<ActionResult<RecipeDTO>> UpdateRecipe(int id, UpdateRecipeDTO updateRecipeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oldRecipeDTO = await _recipeRepository.GetRecipeByIdAsync(id);

            if (!oldRecipeDTO.IsSuccess)
                return NotFound(ErrorResponse.CreateFromError(oldRecipeDTO.Error!));

            if (updateRecipeDTO.Id != oldRecipeDTO.Value?.Id)
                return BadRequest(ErrorResponse.CreateFromError(ErrorMessages.IdsNotMatching));

            var recipeResult = _recipeService.ValidateAndCreateRecipe(oldRecipeDTO.Value, updateRecipeDTO);

            await _recipeRepository.UpdateRecipeAsync(recipeResult.Value!);

            WarningHeaderHelper.AddWarningsToResponse(Response, recipeResult.Warnings);

            return Ok(recipeResult.Value);
        }
    }
}
