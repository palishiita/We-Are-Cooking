using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Entities.Reviews;
using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("recipesapi/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("recipe/{recipeId}")]
        public async Task<IActionResult> GetReviewsByRecipeId(Guid recipeId)
        {
            try
            {
            var reviews = await _reviewService.GetReviewsByRecipeId(recipeId);
            return Ok(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpPost("recipe/{recipeId}")]
        public async Task<IActionResult> AddReview(Guid recipeId, [FromBody] AddReviewRequestDTO dto, [FromHeader] Guid userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newReviewId = await _reviewService.AddReview(dto, userId, recipeId);
                return CreatedAtAction(nameof(AddReview), newReviewId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpDelete("recipe/{recipeId}")]
        public async Task<IActionResult> DeleteReview(Guid recipeId, [FromHeader] Guid userId)
        {
            await _reviewService.DeleteReview(recipeId, userId);
            return NoContent();
        }
    }
}
