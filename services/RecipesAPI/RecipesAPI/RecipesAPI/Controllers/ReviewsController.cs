using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Model.Reviews.Get;
using RecipesAPI.Services.Interfaces;


namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Produces("application/json")]
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

        [HttpGet("{reviewId}")]
        [ProducesResponseType(typeof(GetReviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetReviewDTO>> GetReviewById(
            Guid reviewId, 
            [FromHeader(Name = "X-Uuid")] Guid requestedUserId, 
            CancellationToken ct
            )
        {
            if (requestedUserId == Guid.Empty)
            {
                return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = $"Header 'X-Uuid' is missing or invalid.", Status = StatusCodes.Status400BadRequest });
            }
            try
            {
                var review = await _reviewService.GetReviewById(reviewId, requestedUserId, ct);
                if (review == null)
                {
                    _logger.LogWarning("Review not found with ID: {ReviewId}", reviewId);
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Review with ID {reviewId} not found.", Status = StatusCodes.Status404NotFound });
                }
                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving review with ID: {ReviewId}", reviewId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occurred", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpGet("user")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetReviewDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<IEnumerable<GetReviewDTO>>>> GetReviewsByUserId(
            [FromHeader(Name = "X-Uuid")] Guid userId,
            CancellationToken ct,
            [FromQuery] int pageNumber = DefaultPageNumber,
            [FromQuery] int pageSize = DefaultPageSize
            )
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("GetReviewsByUserId called with empty UserId in header.");
                return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "User ID in header (X-Uuid) is missing or invalid.", Status = StatusCodes.Status400BadRequest });
            }

            if (pageNumber < DefaultPageNumber) pageNumber = DefaultPageNumber;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            try
            {
                var paginatedReviews = await _reviewService.GetReviewsByUserId(userId, pageNumber, pageSize, ct);
                return Ok(paginatedReviews);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found (or no reviews) for User ID: {UserId}", userId);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reviews for User ID: {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occurred", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpGet("recipe/{recipeId}")]
        [ProducesResponseType(typeof(PaginatedResult<IEnumerable<GetReviewDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<IEnumerable<GetReviewDTO>>>> GetReviewsByRecipeId(
            Guid recipeId,
            CancellationToken ct,
            [FromQuery] int pageNumber = DefaultPageNumber,
            [FromQuery] int pageSize = DefaultPageSize
            )
        {
            if (pageNumber < DefaultPageNumber)
            {
                _logger.LogWarning("Invalid PageNumber {RequestedPageNumber} for recipeId {RecipeId}, using default {DefaultPageNumber}.", pageNumber, recipeId, DefaultPageNumber);
                pageNumber = DefaultPageNumber;
            }
            if (pageSize < 1)
            {
                _logger.LogWarning("Invalid PageSize {RequestedPageSize} for recipeId {RecipeId}, using default {DefaultPageSize}.", pageSize, recipeId, DefaultPageSize);
                pageSize = DefaultPageSize;
            }
            else if (pageSize > MaxPageSize)
            {
                _logger.LogWarning("Requested PageSize {RequestedPageSize} for recipeId {RecipeId} exceeds MaxPageSize {MaxPageSize}, capping.", pageSize, recipeId, MaxPageSize);
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
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reviews for recipe ID: {RecipeId}", recipeId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occurred", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpPost("recipe/{recipeId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddReview(Guid recipeId, [FromBody] AddReviewRequestDTO dto, [FromHeader(Name = "X-UserId")] Guid userId, CancellationToken ct)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "User ID in header (X-UserId) is missing or invalid.", Status = StatusCodes.Status400BadRequest });
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var newReviewId = await _reviewService.AddReview(dto, userId, recipeId, ct);
                return CreatedAtAction(nameof(GetReviewById), new { reviewId = newReviewId }, new { reviewId = newReviewId });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Recipe not found for ID: {RecipeId} when attempting to add review.", recipeId);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding a review for recipe ID: {RecipeId}", recipeId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "Database error", Detail = "A database error occurred while saving the review.", Status = StatusCodes.Status500InternalServerError });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a review for recipe ID: {RecipeId}", recipeId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occurred", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError });
            }
        }


        [HttpDelete("recipe/{recipeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReview(Guid recipeId, [FromHeader(Name = "X-UserId")] Guid userId, CancellationToken ct)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "User ID in header (X-UserId) is missing or invalid.", Status = StatusCodes.Status400BadRequest });
            }
            try
            {
                await _reviewService.DeleteReview(recipeId, userId, ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found for deletion or user not authorized. RecipeId: {RecipeId}, UserId: {UserId}", recipeId, userId);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting review. RecipeId: {RecipeId}, UserId: {UserId}", recipeId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "Database error", Detail = "A database error occurred while deleting the review.", Status = StatusCodes.Status500InternalServerError });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a review for recipe ID: {RecipeId} and user ID: {UserId}", recipeId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occurred", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError });
            }
        }
    }
}