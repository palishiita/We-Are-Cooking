using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Exceptions.Duplicates;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [Route("recipesapi/ingredients")]
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

        [HttpGet]
        [Route("ingredient/{ingredientId:guid}")]
        [ProducesResponseType(typeof(GetIngredientDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult GetIngredientById([FromRoute] Guid ingredientId)
        {
            try
            {
                var ingredient = _ingredientService.GetIngredientById(ingredientId);
                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("ingredients")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllFullIngredients([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var ingredient = await _ingredientService.GetAllIngredients(count.Value, page.Value, orderByAsc.Value, sortBy, query);
                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("ingredients/categories")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientWithCategoriesDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllIngredientsWithCategoriesAsync([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var ingredient = await _ingredientService.GetAllIngredientsWithCategories(count.Value, page.Value, orderByAsc.Value, sortBy, query);
                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("ingredient/{ingredientId:guid}/categories")]
        [ProducesResponseType(typeof(GetIngredientWithCategoriesDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult GetFullIngredientById([FromRoute] Guid ingredientId)
        {
            try
            {
                var ingredient = _ingredientService.GetIngredientWithCategoriesById(ingredientId);
                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
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
        [Route("ingredient/categories_ids")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNewIngredientWithCategories([FromBody] AddIngredientWithCategoryIdsDTO ingredientDTO)
        {
            try
            {
                var id = await _ingredientService.AddIngredientWithCategoriesByIds(ingredientDTO);
                return CreatedAtAction(nameof(AddNewIngredientWithCategories), id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ingredient/categories_names")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNewIngredientWithCategoriesByNames([FromBody] AddIngredientWithCategoryNamesDTO ingredientDTO)
        {
            try
            {
                var id = await _ingredientService.AddIngredientWithCategoriesByNames(ingredientDTO);
                return CreatedAtAction(nameof(AddNewIngredientWithCategoriesByNames), id);
            }
            catch (Exception ex)
            {
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

        [HttpGet]
        [Route("ingredient_category")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllIngredientCategories([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query)
        {
            count ??= 10;
            page ??= 0;
            orderByAsc ??= true;

            sortBy = string.IsNullOrEmpty(sortBy) ? string.Empty : sortBy;
            query = string.IsNullOrEmpty(query) ? string.Empty : query;

            try
            {
                var categories = await _ingredientService.GetAllIngredientCategories(count.Value, page.Value, orderByAsc.Value, sortBy, query);

                if (!categories.Data.Any())
                {
                    return NotFound("No ingredient categories matching the given query.");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
