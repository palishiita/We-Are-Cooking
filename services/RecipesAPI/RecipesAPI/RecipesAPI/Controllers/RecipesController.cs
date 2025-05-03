using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Database;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [Route("recipesapi/recipes")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        // temporary for the test
        private readonly RecipeDbContext _recipeDbContext;
        
        private readonly IRecipeService _recipeService;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(RecipeDbContext recipeDb, IRecipeService recipeService, ILogger<RecipesController> logger)
        {
            _recipeDbContext = recipeDb;
            _recipeService = recipeService;
            _logger = logger;
        }


#if DEBUG
        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            if (_recipeDbContext.Database.CanConnect())
            {
                return Ok("Workiiiing :3");
            }
            return NotFound("Not working.... :----(");
        }
#endif

        // should add page count in the response
        [Route("recipes/full")]
        [ProducesResponseType(typeof(IEnumerable<GetFullRecipeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet]
        public IActionResult GetAllRecipesFull([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var recipes = _recipeService.GetAllFullRecipes(count.Value, page.Value, orderByAsc.Value, sortBy, query);

                if (!recipes.Any())
                {
                    return NotFound("No recipes matching the given query.");
                }
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }
        }

        [Route("recipe")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> AddNewRecipeWithIngredientsByIds([FromHeader] Guid userId, [FromBody] AddRecipeWithIngredientIdsDTO recipeDTO)
        {
            try
            {
                var id = await _recipeService.CreateRecipeWithIngredientsByIds(userId, recipeDTO);
                return CreatedAtAction(nameof(AddNewRecipeWithIngredientsByIds), id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }
        }

        // should add page count in the response
        [Route("recipes")]
        [ProducesResponseType(typeof(IEnumerable<GetRecipeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet]
        public IActionResult GetAllRecipes([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var recipes = _recipeService.GetAllRecipes(count.Value, page.Value, orderByAsc.Value, sortBy, query);

                if (!recipes.Any())
                {
                    return NotFound("No recipes matching the given query.");
                }
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }
        }

        [Route("recipe/{recipeId:guid}")]
        [ProducesResponseType(typeof(GetRecipeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RecipeNotFoundException), StatusCodes.Status404NotFound)]
        [HttpGet]
        public IActionResult GetRecipeById(Guid recipeId)
        {
            try
            {
                var recipe = _recipeService.GetRecipeById(recipeId);
                return Ok(recipe);
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }

        }

        [Route("recipe/{recipeId:guid}/full")]
        [ProducesResponseType(typeof(GetFullRecipeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RecipeNotFoundException), StatusCodes.Status404NotFound)]
        [HttpGet]
        public IActionResult GetFullRecipeById(Guid recipeId)
        {
            try
            {
                var recipe = _recipeService.GetFullRecipeById(recipeId);
                return Ok(recipe);
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }

        }

        [Route("recipe/{recipeId:guid}/ingredient_categories")]
        [ProducesResponseType(typeof(GetRecipeWithIngredientsAndCategoriesDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet]
        public IActionResult GetRecipeWithIngredientCategoriesById([FromRoute] Guid recipeId)
        {
            try
            {
                var recipe = _recipeService.GetRecipeWithIngredientsAndCategories(recipeId);

                if (recipe == null)
                {
                    return NotFound();
                }
                return Ok(recipe);
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }
        }

        [Route("recipe/{recipeId:guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ElementNotFoundException), StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<IActionResult> DeleteRecipeById([FromRoute] Guid recipeId)
        {
            try
            {
                await _recipeService.RemoveRecipeById(recipeId);
                return NoContent();
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }
        }

        [Route("recipe/{recipeId:guid}/ingredient")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ElementNotFoundException), StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<IActionResult> DeleteIngredientFromRecipeById([FromRoute] Guid recipeId, [FromBody] Guid ingredientId)
        {
            try
            {
                await _recipeService.RemoveIngredientFromRecipe(recipeId, ingredientId);
                return NoContent();
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }
        }


        [Route("recipe/{recipeId:guid}/ingredients")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ElementNotFoundException), StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<IActionResult> DeleteIngredientsFromRecipeById([FromRoute] Guid recipeId, [FromBody] IEnumerable<Guid> ingredientIds)
        {
            try
            {
                await _recipeService.RemoveIngredientsFromRecipe(recipeId, ingredientIds);
                return NoContent();
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex);
            }
        }
    }
}
