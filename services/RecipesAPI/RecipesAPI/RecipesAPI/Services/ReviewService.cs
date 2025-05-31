using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Reviews;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Model.Reviews.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly RecipeDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(RecipeDbContext context, IUserInfoService userInfoService, ILogger<ReviewService> logger)
        {
            _context = context;
            _userInfoService = userInfoService;
            _logger = logger;
        }

        public async Task<GetReviewDTO?> GetReviewById(Guid reviewId, CancellationToken ct)
        {
            _logger.LogInformation("Fetching review by ID: {ReviewId}", reviewId);
            var review = await _context.Reviews
                .Where(r => r.Id == reviewId)
                .Include(r => r.ReviewPhotos).ThenInclude(rp => rp.PhotoUrl)
                .FirstOrDefaultAsync(ct);

            if (review == null)
            {
                _logger.LogWarning("Review with ID: {ReviewId} not found.", reviewId);
                return null;
            }

            CommonUserDataDTO userInfo;
            try
            {
                userInfo = await _userInfoService.GetUserById(review.UserId)
                           ?? new CommonUserDataDTO(review.UserId, "N/A", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user info for UserId {UserId} while getting review {ReviewId}. Using fallback.", review.UserId, reviewId);
                userInfo = new CommonUserDataDTO(review.UserId, "N/A", null);
            }

            return new GetReviewDTO(
                review.Id,
                review.RecipeId,
                userInfo,
                (float)review.Rating,
                review.Description,
                review.HasPhotos,
                review.ReviewPhotos.Select(p => p.PhotoUrl.Url).ToList()
            );
        }

        public async Task<PaginatedResult<IEnumerable<GetReviewDTO>>> GetReviewsByUserId(Guid userId, int pageNumber, int pageSize, CancellationToken ct)
        {
            _logger.LogInformation("Fetching reviews for UserId: {UserId}, Page: {PageNumber}, PageSize: {PageSize}", userId, pageNumber, pageSize);

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            const int MaxPageSize = 50;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            CommonUserDataDTO userInfoDto;
            try
            {
                var fetchedUserInfo = await _userInfoService.GetUserById(userId);
                userInfoDto = fetchedUserInfo ?? new CommonUserDataDTO(userId, "N/A", null);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with ID: {UserId} when fetching their reviews.", userId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user info for UserId {UserId}. Using fallback.", userId);
                userInfoDto = new CommonUserDataDTO(userId, "N/A", null);
            }

            IQueryable<Review> baseQuery = _context.Reviews.Where(r => r.UserId == userId);
            int totalElements = await baseQuery.CountAsync(ct);
            _logger.LogDebug("Total reviews found for UserId {UserId}: {TotalElements}", userId, totalElements);

            List<GetReviewDTO> reviewsData;
            if (totalElements > 0)
            {
                reviewsData = await baseQuery
                    .Include(r => r.ReviewPhotos)
                        .ThenInclude(rp => rp.PhotoUrl)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new GetReviewDTO(
                        r.Id,
                        r.RecipeId,
                        userInfoDto,
                        (float)r.Rating,
                        r.Description,
                        r.HasPhotos,
                        r.ReviewPhotos.Select(p => p.PhotoUrl.Url)
                    ))
                    .ToListAsync(ct);
            }
            else
            {
                reviewsData = new List<GetReviewDTO>();
            }

            var totalPages = (totalElements > 0 && pageSize > 0) ? (int)Math.Ceiling(totalElements / (double)pageSize) : 0;

            return new PaginatedResult<IEnumerable<GetReviewDTO>>
            {
                Data = reviewsData,
                TotalElements = totalElements,
                TotalPages = totalPages,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResult<IEnumerable<GetReviewDTO>>> GetReviewsByRecipeId(Guid recipeId, CancellationToken ct, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching reviews for RecipeId: {RecipeId}, Page: {PageNumber}, PageSize: {PageSize}", recipeId, pageNumber, pageSize);

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            const int MaxPageSize = 50;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            IQueryable<Review> baseQuery = _context.Reviews
                .Where(r => r.RecipeId == recipeId);

            int totalElements = await baseQuery.CountAsync(ct);
            _logger.LogDebug("Total reviews found for RecipeId {RecipeId}: {TotalElements}", recipeId, totalElements);

            List<GetReviewDTO> data;
            if (totalElements > 0)
            {
                var reviewsOnPageTemp = await baseQuery
                    .Include(r => r.ReviewPhotos)
                        .ThenInclude(rp => rp.PhotoUrl)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new
                    {
                        r.Id,
                        r.RecipeId,
                        r.UserId,
                        r.Rating,
                        r.Description,
                        r.HasPhotos,
                        PhotoUrls = r.ReviewPhotos.Select(p => p.PhotoUrl.Url)
                    })
                    .ToListAsync(ct);

                var userIds = reviewsOnPageTemp.Select(r => r.UserId).Distinct().ToList();
                var userDtoMap = new Dictionary<Guid, CommonUserDataDTO>();

                if (userIds.Any())
                {
                    var userInfoTasks = userIds.Select(async uId => {
                        try { return await _userInfoService.GetUserById(uId); }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error fetching user info for UserId {UserId} in GetReviewsByRecipeId. Using fallback.", uId);
                            return new CommonUserDataDTO(uId, "N/A", null);
                        }
                    }).ToList();
                    var fetchedUserInfos = await Task.WhenAll(userInfoTasks);
                    userDtoMap = fetchedUserInfos.Where(u => u != null).ToDictionary(u => u.UserId);
                }

                data = reviewsOnPageTemp.Select(tempReview => {
                    userDtoMap.TryGetValue(tempReview.UserId, out var userInfo);
                    var finalUserInfo = userInfo ?? new CommonUserDataDTO(tempReview.UserId, "N/A", null);
                    return new GetReviewDTO(
                        tempReview.Id, tempReview.RecipeId, finalUserInfo,
                        (float)tempReview.Rating, tempReview.Description, tempReview.HasPhotos,
                        tempReview.PhotoUrls
                    );
                }).ToList();
            }
            else
            {
                data = new List<GetReviewDTO>();
            }

            var totalPages = (totalElements > 0 && pageSize > 0) ? (int)Math.Ceiling(totalElements / (double)pageSize) : 0;

            return new PaginatedResult<IEnumerable<GetReviewDTO>>
            {
                Data = data,
                TotalElements = totalElements,
                TotalPages = totalPages,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Guid> AddReview(AddReviewRequestDTO dto, Guid userId, Guid recipeId, CancellationToken ct)
        {
            _logger.LogInformation("Adding review for RecipeId: {RecipeId} by UserID: {UserId}", recipeId, userId);
            var recipeExists = await _context.Recipes.AnyAsync(r => r.Id == recipeId, ct);
            if (!recipeExists)
            {
                _logger.LogWarning("Attempted to add review to non-existent RecipeId: {RecipeId}", recipeId);
                throw new KeyNotFoundException($"Recipe with ID {recipeId} not found.");
            }

            var review = new Review
            {
                RecipeId = recipeId,
                UserId = userId,
                Rating = dto.Rating,
                Description = dto.Description,
                HasPhotos = (dto.Photos != null && dto.Photos.Any())
            };

            _context.Reviews.Add(review);

            try
            {
                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("Review added successfully with ID: {ReviewId}", review.Id);
                return review.Id;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding review for RecipeId: {RecipeId}", recipeId);
                throw;
            }
        }

        public async Task DeleteReview(Guid recipeId, Guid userId, CancellationToken ct)
        {
            _logger.LogInformation("Attempting to delete review for RecipeId: {RecipeId} by UserID: {UserId}", recipeId, userId);
            var review = await _context.Reviews
                .Include(r => r.ReviewPhotos)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId, ct);

            if (review == null)
            {
                _logger.LogWarning("Review not found for deletion. RecipeId: {RecipeId}, UserId: {UserId}", recipeId, userId);
                throw new KeyNotFoundException("Review not found or you're not authorized to delete it.");
            }

            if (review.ReviewPhotos.Any())
            {
                _context.ReviewPhotos.RemoveRange(review.ReviewPhotos);
            }
            _context.Reviews.Remove(review);

            try
            {
                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("Review deleted successfully. RecipeId: {RecipeId}, UserId: {UserId}", recipeId, userId);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while deleting review. RecipeId: {RecipeId}, UserId: {UserId}", recipeId, userId);
                throw;
            }
        }
    }
}