using Microsoft.AspNetCore.Mvc;
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
            var reviews = await _reviewService.GetReviewsByRecipeId(recipeId);
            return Ok(reviews);
        }

        [HttpPost("description/recipe/{recipeId}/user/{userId}")]
        public async Task<IActionResult> AddReviewWithDescription(Guid recipeId, Guid userId, [FromBody] AddReviewWithDescriptionDTO dto)
        {
            var newReviewId = await _reviewService.AddReviewWithDescription(dto, userId, recipeId);
            return StatusCode(StatusCodes.Status201Created, new { reviewId = newReviewId });
        }

        [HttpPost("photos/recipe/{recipeId}/user/{userId}")]
        public async Task<IActionResult> AddReviewWithPhotos(Guid recipeId, Guid userId, [FromBody] AddReviewWithPhotosDTO dto)
        {
            var newReviewId = await _reviewService.AddReviewWithPhotos(dto, userId, recipeId);
            return StatusCode(StatusCodes.Status201Created, new { reviewId = newReviewId });
        }

        [HttpDelete("recipe/{recipeId}/user/{userId}")]
        public async Task<IActionResult> DeleteReview(Guid recipeId, Guid userId)
        {
            await _reviewService.DeleteReview(recipeId, userId);
            return NoContent();
        }
    }
}
