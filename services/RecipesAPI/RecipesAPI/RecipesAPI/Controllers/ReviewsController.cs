using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Entities.Reviews;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Model.Reviews.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("recipesapi/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("recipe/{recipeId}")]
        public async Task<IActionResult> GetReviewsByRecipeId(Guid recipeId, [FromQuery] PaginationParameters paginationParameters, CancellationToken ct)
        {
            try
            {
            PaginatedResult<IEnumerable<GetReviewDTO>> reviews = await _reviewService.GetReviewsByRecipeId(recipeId, paginationParameters, ct);
                return Ok(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Recipe not found for ID: {RecipeId}", recipeId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reviews for recipe ID: {RecipeId}", recipeId);
                return BadRequest(ex.Message);
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
