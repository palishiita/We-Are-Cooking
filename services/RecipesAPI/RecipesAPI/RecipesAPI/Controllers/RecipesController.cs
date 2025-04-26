using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Database;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [Route("recipesapi/[controller]")]
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

        // should add page count in the response
        [Route("recipes/full")]
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

                if (recipes.Count() < 1)
                {
                    return NotFound("No recipe present in the database.");
                }
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        // should add page count in the response
        [Route("recipes")]
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

                if (recipes.Count() < 1)
                {
                    return NotFound("No recipe present in the database.");
                }
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [Route("recipe/{recipeId:guid}")]
        [HttpGet]
        public IActionResult GetRecipeById(Guid recipeId)
        {
            try
            {
                var recipe = _recipeService.GetRecipeById(recipeId);

                if (recipe == null)
                {
                    return NotFound();
                }
                return Ok(recipe);
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogInformation(ex, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }

        }

        [Route("recipe/{recipeId:guid}/full")]
        [HttpGet]
        public IActionResult GetFullRecipeById(Guid recipeId)
        {
            try
            {
                var recipe = _recipeService.GetFullRecipeById(recipeId);

                if (recipe == null)
                {
                    return NotFound();
                }
                return Ok(recipe);
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogInformation(ex, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }

        }

        [Route("recipe/{recipeId:guid}/ingredient_categories")]
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
                _logger.LogInformation(ex, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

    }
}
