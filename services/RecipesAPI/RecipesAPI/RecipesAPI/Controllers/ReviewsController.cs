using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Entities.Reviews;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Model.Reviews.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 50;
        private const int DefaultPageNumber = 1;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("recipe/{recipeId}")]
        public async Task<ActionResult<PaginatedResult<IEnumerable<GetReviewDTO>>>> GetReviewsByRecipeId(
            Guid recipeId, CancellationToken ct,
            [FromQuery] int pageNumber = DefaultPageNumber,
            [FromQuery] int pageSize = DefaultPageSize)
        {
            if (pageNumber <= 0)
            {
                _logger.LogWarning("Invalid PageNumber {RequestedPageNumber} requested for recipeId {RecipeId}, defaulting to {DefaultPageNumber}.", pageNumber, recipeId, DefaultPageNumber);
                pageNumber = DefaultPageNumber;
            }

            if (pageSize <= 0)
            {
                _logger.LogWarning("Invalid PageSize {RequestedPageSize} requested for recipeId {RecipeId}, defaulting to {DefaultPageSize}.", pageSize, recipeId, DefaultPageSize);
                pageSize = DefaultPageSize;
            }
            else if (pageSize > MaxPageSize)
            {
                _logger.LogWarning("Requested PageSize {RequestedPageSize} for recipeId {RecipeId} exceeds MaxPageSize {MaxPageSize}, capping at MaxPageSize.", pageSize, recipeId, MaxPageSize);
                pageSize = MaxPageSize;
            }

            try
            {
                PaginatedResult<IEnumerable<GetReviewDTO>> reviews = await _reviewService.GetReviewsByRecipeId(recipeId, ct, pageNumber, pageSize);
                return Ok(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Recipe not found for ID: {RecipeId} when fetching reviews.", recipeId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reviews for recipe ID: {RecipeId}", recipeId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpPost("recipe/{recipeId}")]
        public async Task<IActionResult> AddReview(Guid recipeId, [FromBody] AddReviewRequestDTO dto, [FromHeader] Guid userId, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newReviewId = await _reviewService.AddReview(dto, userId, recipeId, ct);
                return CreatedAtAction(nameof(AddReview), newReviewId);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Recipe not found for ID: {RecipeId}", recipeId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a review for recipe ID: {RecipeId}", recipeId);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("recipe/{recipeId}")]
        public async Task<IActionResult> DeleteReview(Guid recipeId, [FromHeader] Guid userId, CancellationToken ct)
        {
            try
            {
                await _reviewService.DeleteReview(recipeId, userId, ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Review not found for recipe ID: {RecipeId} and user ID: {UserId}", recipeId, userId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a review for recipe ID: {RecipeId} and user ID: {UserId}", recipeId, userId);
                return BadRequest(ex.Message);
            }
        }
    }
}
