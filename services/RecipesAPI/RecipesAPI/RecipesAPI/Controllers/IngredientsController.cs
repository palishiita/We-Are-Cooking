using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Units.Get;
using RecipesAPI.Model.Units.Request;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    //[Route("api/ingredients")]
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult GetIngredientById([FromRoute] Guid ingredientId, CancellationToken ct)
        {
            try
            {
                var ingredient = _ingredientService.GetIngredientById(ingredientId, ct);
                return Ok(ingredient);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (IngredientNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ingredients")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllFullIngredients([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
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
                var ingredient = await _ingredientService.GetAllIngredients(count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);
                return Ok(ingredient);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllIngredientsWithCategoriesAsync([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
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
                var ingredient = await _ingredientService.GetAllIngredientsWithCategories(count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);
                return Ok(ingredient);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFullIngredientById([FromRoute] Guid ingredientId, CancellationToken ct)
        {
            try
            {
                var ingredient = await _ingredientService.GetIngredientWithCategoriesById(ingredientId, ct);
                return Ok(ingredient);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
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
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllIngredientCategories([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
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
                var categories = await _ingredientService.GetAllIngredientCategories(count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!categories.Data.Any())
                {
                    return NotFound("No ingredient categories matching the given query.");
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
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("units")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetUnitDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUnits([FromQuery] int? count, [FromQuery] int? page, [FromQuery] bool? orderByAsc, [FromQuery] string? sortBy, [FromQuery] string? query, CancellationToken ct)
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
                var units = await _ingredientService.GetAllUnits(count.Value, page.Value, orderByAsc.Value, sortBy, query, ct);

                if (!units.Data.Any())
                {
                    return NotFound("No units matching the given query.");
                }
                return Ok(units);
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
        [Route("unit/{unitId:guid}")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUnitById([FromRoute] Guid unitId, CancellationToken ct)
        {
            try
            {
                var unit = await _ingredientService.GetUnitById(unitId, ct);
                return Ok(unit);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (UnitNotFoundException ex)
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


        [HttpPost]
        [Route("ingredient/units")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetIngredientDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TranslateIngredientUnit([FromBody] RequestUnitQuantityTranslationDTO request, CancellationToken ct)
        {
            try
            {
                var translation = await _ingredientService.GetTranslatedUnitQuantities(request, ct);
                return Ok(translation);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return NoContent();
            }
            catch (UnitTranslationNotFoundException ex)
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
