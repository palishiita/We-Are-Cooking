using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Exceptions.Duplicates;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [Route("recipesapi/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private ILogger<IngredientsController> _logger;
        
        private readonly IIngredientService _ingredientService;

        public IngredientsController(ILogger<IngredientsController> logger, IIngredientService ingredientService)
        {
            _logger = logger;

            _ingredientService = ingredientService;
        }

        [HttpPost]
        [Route("ingredient")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNewIngredient([FromBody] AddIngredientDTO ingredientDTO)
        {
            try
            {
                var id = await _ingredientService.AddIngredient(ingredientDTO);
                return CreatedAtAction(nameof(AddNewIngredient), id);
            }
            catch (Exception ex) { 
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Route("ingredient_category")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNewIngredientCategory([FromBody] AddIngredientCategoryDTO ingredientDTO)
        {
            try
            {
                var id = await _ingredientService.AddIngredientCategory(ingredientDTO);
                return CreatedAtAction(nameof(AddNewIngredientCategory), id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
