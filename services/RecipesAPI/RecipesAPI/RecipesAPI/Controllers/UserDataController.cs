using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
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


        [Route("cookbook")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<IActionResult> GetAllRecipesFull([FromHeader] Guid userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, [FromQuery] bool? showOnlyFavorites)
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
    }
}
