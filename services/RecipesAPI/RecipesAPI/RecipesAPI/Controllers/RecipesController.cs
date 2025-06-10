using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Database;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.Recipes.Update;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [Route("api/recipes")]
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

        [Route("recipes/full")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetFullRecipeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Recipes with full ingredient data.")]
        [HttpGet]
        public async Task<IActionResult> GetAllRecipesFull([FromHeader(Name = "X-Uuid")] string userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
        {
            if (count == null || count < 1)
            {
                count = 10;
            }
            if (page == null || page < 1)
            {
                page = 1;
            }
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var parsedId = new Guid(userId);
                var recipes = await _recipeService.GetAllFullRecipes(parsedId, count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!recipes.Data.Any())
                {
                    return NotFound("No recipes matching the given query.");
                }
                return Ok(recipes);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [Route("recipes/full/{selectedUserId:guid}")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetFullRecipeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Recipes with full ingredient data.")]
        [HttpGet]
        public async Task<IActionResult> GetAllRecipesFull([FromHeader(Name = "X-Uuid")] string userId, [FromRoute] Guid selectedUserId, [FromQuery] int? count, [FromQuery] int? page, CancellationToken ct)
        {
            if (count == null || count < 1)
            {
                count = 10;
            }
            if (page == null || page < 1)
            {
                page = 1;
            }

            try
            {
                var parsedId = new Guid(userId);
                var recipes = await _recipeService.GetUserFullRecipes(parsedId, selectedUserId, count.Value, page.Value, ct);

                if (!recipes.Data.Any())
                {
                    return NotFound("No recipes matching the given query.");
                }
                return Ok(recipes);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [Route("recipe")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> AddNewRecipeWithIngredientsByIds([FromHeader(Name = "X-Uuid")] string userId, [FromBody] AddRecipeWithIngredientsDTO recipeDTO, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                var id = await _recipeService.CreateRecipeWithIngredientsByIds(parsedId, recipeDTO, ct);
                return CreatedAtAction(nameof(AddNewRecipeWithIngredientsByIds), id);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }


        [Route("recipe/{recipeId:guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPut]
        public async Task<IActionResult> SetRecipeIngredients([FromHeader(Name = "X-Uuid")] string userId, [FromRoute] Guid recipeId, [FromBody] AddRecipeWithIngredientsDTO recipeDTO, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _recipeService.UpdateRecipe(parsedId, recipeId, recipeDTO, ct);
                return Ok();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        // should add page count in the response
        [Route("recipes")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetRecipeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Recipes only with name and description.")]
        [HttpGet]
        public async Task<IActionResult> GetAllRecipes([FromHeader(Name = "X-Uuid")] string userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
        {
            if (count == null || count < 1)
            {
                count = 10;
            }
            if (page == null || page < 1)
            {
                page = 1;
            }
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var parsedId = new Guid(userId);
                var recipes = await _recipeService.GetAllRecipes(parsedId, count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!recipes.Data.Any())
                {
                    return NotFound("No recipes matching the given query.");
                }
                return Ok(recipes);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [Route("recipe/{recipeId:guid}")]
        [ProducesResponseType(typeof(GetRecipeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Recipe name and description by id.")]
        [HttpGet]
        public async Task<IActionResult> GetRecipeById([FromHeader(Name = "X-Uuid")] string userId, Guid recipeId, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                var recipe = await _recipeService.GetRecipeById(parsedId, recipeId, ct);
                return Ok(recipe);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }

        }

        [Route("recipe/{recipeId:guid}/full")]
        [ProducesResponseType(typeof(GetFullRecipeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Recipe with full ingredient data by id.")]
        [HttpGet]
        public async Task<IActionResult> GetFullRecipeByIdAsync([FromHeader(Name = "X-Uuid")] string userId, Guid recipeId, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                var recipe = await _recipeService.GetFullRecipeById(parsedId, recipeId, ct);
                return Ok(recipe);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }

        }

        [Route("recipe/{recipeId:guid}/ingredient_categories")]
        [ProducesResponseType(typeof(GetRecipeWithIngredientsAndCategoriesDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Recipes with full ingredient data, each with the connected categories.")]
        [HttpGet]
        public async Task<IActionResult> GetRecipeWithIngredientCategoriesByIdAsync([FromHeader(Name = "X-Uuid")] string userId, [FromRoute] Guid recipeId, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                var recipe = await _recipeService.GetRecipeWithIngredientsAndCategories(parsedId, recipeId, ct);

                if (recipe == null)
                {
                    return NotFound();
                }
                return Ok(recipe);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogError(ex.Message, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [Route("recipe/{recipeId:guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<IActionResult> DeleteRecipeById([FromHeader(Name = "X-Uuid")] string userId, [FromRoute] Guid recipeId, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _recipeService.RemoveRecipeById(parsedId, recipeId, ct);
                return NoContent();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        //[Route("recipe/{recipeId:guid}")]
        //[ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        //[HttpPut]
        //public async Task<IActionResult> UpdateRecipeById([FromHeader(Name = "X-Uuid")] Guid userId, [FromRoute] Guid recipeId, [FromBody] UpdateRecipeDTO recipeDTO, CancellationToken ct)
        //{
        //    try
        //    {
        //        await _recipeService.UpdateRecipeNameById(userId, recipeId, recipeDTO, ct);
        //        return Ok();
        //    }
        //    catch (OperationCanceledException ex)
        //    {
        //        _logger.LogInformation(ex, ex.Message);
        //        return NoContent();
        //    }
        //    catch (RecipeNotFoundException ex)
        //    {
        //        _logger.LogError(ex, $"Exception: {ex.Message}.");
        //        return NotFound(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Exception: {ex.Message}.");
        //        return BadRequest(ex.Message);
        //    }
        //}

        [Route("recipe/{recipeId:guid}/ingredients")]
        [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost]
        public async Task<IActionResult> AddIngredientsToRecipeById([FromHeader(Name = "X-Uuid")] string userId, [FromRoute] Guid recipeId, [FromBody] AddIngredientRangeToRecipeDTO ingredients, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                var addedIngredients = await _recipeService.AddIngredientsToRecipeById(parsedId, recipeId, ingredients, ct);
                return CreatedAtAction(nameof(AddIngredientsToRecipeById), addedIngredients);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (RecipeNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [Route("recipe/{recipeId:guid}/ingredients")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<IActionResult> DeleteIngredientsFromRecipeById([FromHeader(Name = "X-Uuid")] string userId, [FromRoute] Guid recipeId, [FromBody] IEnumerable<Guid> ingredientIds, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _recipeService.RemoveIngredientsFromRecipe(parsedId, recipeId, ingredientIds, ct);
                return NoContent();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (ElementNotFoundException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
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
