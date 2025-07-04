﻿using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
using RecipesAPI.Model.UserData.Fridge.Add;
using RecipesAPI.Model.UserData.Fridge.Get;
using RecipesAPI.Services.Interfaces;
using System.Collections.Generic;

namespace RecipesAPI.Controllers
{
    //[Route("api/userdata")]
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Get the recipes from user cookbook.")]
        public async Task<IActionResult> GetCookbookRecipes([FromHeader(Name = "X-Uuid")] string userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, [FromQuery] bool? showOnlyFavorites, CancellationToken ct)
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

            showOnlyFavorites ??= false;

            try
            {
                var parsedId = new Guid(userId);
                var recipes = await _userDataService.GetFullUserCookbook(parsedId, count.Value, page.Value, orderByAsc.Value, sortBy, query, showOnlyFavorites.Value, ct);

                if (!recipes.Data.Any())
                {
                    return NotFound("No recipes in cookbook matching the given query.");
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

        [HttpPost]
        [Route("cookbook/recipe")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Add a recipe to user cookbook.")]
        public async Task<IActionResult> AddRecipeToCookbook([FromHeader(Name = "X-Uuid")] string userId, [FromBody] AddRecipeToCookbookDTO recipeDTO, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _userDataService.AddRecipeToCookbook(parsedId, recipeDTO, ct);
                return CreatedAtAction(nameof(AddRecipeToCookbook), recipeDTO.RecipeId);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("cookbook/recipes/check")]
        [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Check if recipes with given ids are in user cookbook.")]
        public IActionResult GetPresentRecipesInCookbook([FromHeader(Name = "X-Uuid")] string userId, [FromBody] IEnumerable<Guid> recipeIds)
        {
            try
            {
                var parsedId = new Guid(userId);
                var data = _userDataService.GetIdsOfPresentRecipesInCookbook(parsedId, recipeIds);

                if (!data.Any()) {
                    return NotFound("No recipe of given id is in the cookbook.");
                }
                return Ok(data);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
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
        public async Task<IActionResult> RemoveRecipesFromCookbook([FromHeader(Name = "X-Uuid")] string userId, [FromBody] IEnumerable<Guid> recipeIds, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _userDataService.RemoveRecipesFromCookbook(parsedId, recipeIds, ct);
                return NoContent();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Get ingredients in the fridge.")]
        public async Task<IActionResult> GetFridgeIngredients([FromHeader(Name = "X-Uuid")] string userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, [FromQuery] bool? showOnlyFavorites, CancellationToken ct)
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

            showOnlyFavorites ??= false;

            try
            {
                var parsedId = new Guid(userId);
                var ingredients = await _userDataService.GetFridgeIngredients(parsedId, count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!ingredients.Data.Any())
                {
                    return NotFound("No ingredients in the fridge matching the given query.");
                }
                return Ok(ingredients);
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

        [HttpGet]
        [Route("fridge/recipes")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetFullRecipeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [EndpointDescription("Get the recipes available from ingredients in the fridge.")]
        public async Task<IActionResult> GetFridgePossibleRecipes([FromHeader(Name = "X-Uuid")] string userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
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
                var ingredients = await _userDataService.GetRecipesAvailableWithFridge(parsedId, count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!ingredients.Data.Any())
                {
                    return NotFound("No recipes can be formed, from the ingredients in the frigde, matching the given query.");
                }
                return Ok(ingredients);
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

        [HttpPut]
        [Route("fridge/ingredients")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Set the ingredients in the fridge as given.")]
        public async Task<IActionResult> SetFridgeIngredients([FromHeader(Name = "X-Uuid")] string userId, [FromBody] IEnumerable<SetIngredientQuantityDTO> ingredientDTOs, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _userDataService.SetFridgeIngredients(parsedId, ingredientDTOs, ct);
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

        [HttpPut]
        [Route("fridge/ingredients/recipe/{recipeId:guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Set the ingredients in the fridge as given.")]
        public async Task<IActionResult> SetFridgeIngredients([FromHeader(Name = "X-Uuid")] string userId, [FromRoute] Guid recipeId, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _userDataService.RemoveUsedIngredientsInRecipe(parsedId, recipeId, ct);
                return Ok();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: {ex.Message}.", ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Obsolete]
        [Route("restrictions/categories")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Get the restricted categories of the user.")]
        public async Task<IActionResult> GetUserRestrictedCategories([FromHeader(Name = "X-Uuid")] string userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
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
                var categories = await _userDataService.GetUserRestrictedCategories(parsedId, count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!categories.Data.Any())
                {
                    return NotFound("No restricted categories matching the given query.");
                }
                return Ok(categories);
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


        [HttpGet]
        [Obsolete]
        [Route("restrictions/ingredients")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Get the restricted ingredients from restricted categories of the user.")]
        public async Task<IActionResult> GetUserRestrictedIngredients([FromHeader(Name = "X-Uuid")] string userId, [FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
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
                var categories = await _userDataService.GetUserRestrictedIngredients(parsedId, count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!categories.Data.Any())
                {
                    return NotFound("No restricted ingredients matching the given query.");
                }
                return Ok(categories);
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

        [HttpPost]
        [Obsolete]
        [Route("restrictions/categories")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Add the categories to restricted categories of the user.")]
        public async Task<IActionResult> AddUserRestrictedCategories([FromHeader(Name = "X-Uuid")] string userId, [FromBody] IEnumerable<Guid> categoryIds, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _userDataService.AddUserRestrictedCategories(parsedId, categoryIds, ct);
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

        [HttpDelete]
        [Obsolete]
        [Route("restrictions/categories")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [EndpointDescription("Add the categories to restricted categories of the user.")]
        public async Task<IActionResult> RemoveUserRestrictedCategories([FromHeader(Name = "X-Uuid")] string userId, [FromBody] IEnumerable<Guid> categoryIds, CancellationToken ct)
        {
            try
            {
                var parsedId = new Guid(userId);
                await _userDataService.RemoveUserRestrictedCategories(parsedId, categoryIds, ct);
                return NoContent();
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
    }
}
