using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
using RecipesAPI.Model.UserData.Fridge.Add;
using RecipesAPI.Model.UserData.Fridge.Get;
using RecipesAPI.Services;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [Route("recipesapi/userdata")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly ILogger<UserDataController> _logger;
        private readonly IUserDataService _userDataService;

        public UserDataController(ILogger<UserDataController> logger, IUserDataService userDataService)
        {
            _logger = logger;
            _userDataService = userDataService;
        }

        [HttpGet]
        [Route("cookbook")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Get the recipes from user cookbook.")]
        public async Task<IActionResult> GetCookbookRecipes([FromHeader] Guid userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, [FromQuery] bool? showOnlyFavorites)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            showOnlyFavorites ??= false;

            try
            {
                var recipes = await _userDataService.GetFullUserCookbook(userId, count.Value, page.Value, orderByAsc.Value, sortBy, query, showOnlyFavorites.Value);

                if (!recipes.Data.Any())
                {
                    return NotFound("No recipes in cookbook matching the given query.");
                }
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("cookbook/recipe")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Add a recipe to user cookbook.")]
        public async Task<IActionResult> AddRecipeToCookbook([FromHeader] Guid userId,  [FromBody] AddRecipeToCookbookDTO recipeDTO)
        {
            try
            {
                await _userDataService.AddRecipeToCookbook(userId, recipeDTO);
                return CreatedAtAction(nameof(AddRecipeToCookbook), recipeDTO.RecipeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("cookbook/recipe")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Remove given recipes from user cookbook.")]
        public async Task<IActionResult> RemoveRecipesFromCookbook([FromHeader] Guid userId, [FromBody] IEnumerable<Guid> recipeIds)
        {
            try
            {
                await _userDataService.RemoveRecipesFromCookbook(userId, recipeIds);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("fridge/ingredients")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Get ingredients in the fridge.")]
        public async Task<IActionResult> GetFridgeIngredients([FromHeader] Guid userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, [FromQuery] bool? showOnlyFavorites)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            showOnlyFavorites ??= false;

            try
            {
                var ingredients = await _userDataService.GetFridgeIngredients(userId, count.Value, page.Value, orderByAsc.Value, sortBy, query);

                if (!ingredients.Data.Any())
                {
                    return NotFound("No recipes in cookbook matching the given query.");
                }
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("fridge/recipes")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetFullRecipeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Get the recipes available from ingredients in the fridge.")]
        public async Task<IActionResult> GetFridgePossibleRecipes([FromHeader] Guid userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var ingredients = await _userDataService.GetRecipesAvailableWithFridge(userId, count.Value, page.Value, orderByAsc.Value, sortBy, query);

                if (!ingredients.Data.Any())
                {
                    return NotFound("No recipes in cookbook matching the given query.");
                }
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("fridge/ingredients")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Set the ingredients in the fridge as given.")]
        public async Task<IActionResult> SetFridgeIngredients([FromHeader] Guid userId, [FromBody] IEnumerable<SetIngredientQuantityDTO> ingredientDTOs)
        {
            try
            {
                await _userDataService.SetFridgeIngredients(userId, ingredientDTOs);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("restrictions/categories")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Get the restricted categories of the user.")]
        public async Task<IActionResult> GetUserRestrictedCategories([FromHeader] Guid userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var categories = await _userDataService.GetUserRestrictedCategories(userId, count.Value, page.Value, orderByAsc.Value, sortBy, query);

                if (!categories.Data.Any())
                {
                    return NotFound("No restricted categories matching the given query.");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("restrictions/ingredients")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Get the restricted ingredients from restricted categories of the user.")]
        public async Task<IActionResult> GetUserRestrictedIngredients([FromHeader] Guid userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var categories = await _userDataService.GetUserRestrictedIngredients(userId, count.Value, page.Value, orderByAsc.Value, sortBy, query);

                if (!categories.Data.Any())
                {
                    return NotFound("No restricted ingredients matching the given query.");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("restrictions/categories")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Add the categories to restricted categories of the user.")]
        public async Task<IActionResult> AddUserRestrictedCategories([FromHeader] Guid userId, [FromBody] IEnumerable<Guid> categoryIds)
        {
            try
            {
                await _userDataService.AddUserRestrictedCategories(userId, categoryIds);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("restrictions/categories")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Add the categories to restricted categories of the user.")]
        public async Task<IActionResult> RemoveUserRestrictedCategories([FromHeader] Guid userId, [FromBody] IEnumerable<Guid> categoryIds)
        {
            try
            {
                await _userDataService.RemoveUserRestrictedCategories(userId, categoryIds);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }
    }
}
