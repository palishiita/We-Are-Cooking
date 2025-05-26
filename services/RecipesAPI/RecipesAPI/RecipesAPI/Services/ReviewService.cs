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

        public ReviewService(RecipeDbContext context, IUserInfoService userInfoService)
        {
            _context = context;
            _userInfoService = userInfoService;
        }

        public async Task<PaginatedResult<IEnumerable<GetReviewDTO>>> GetReviewsByRecipeId(Guid recipeId, CancellationToken ct, int pageNumber, int pageSize)
        {

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<Review> baseQuery = _context.Reviews.Where(r => r.RecipeId == recipeId);
            int totalElements = await baseQuery.CountAsync(ct);

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
            var userInfoTasks = userIds.ToDictionary(id => id, id => _userInfoService.GetUserById(id));
            var userResults = new Dictionary<Guid, CommonUserDataDTO?>();

            foreach (var userId in userIds)
            {
                try
                {
                    if (userInfoTasks.TryGetValue(userId, out var task))
                    {
                        userResults[userId] = await task;
                    }
                    else
                    {
                        userResults[userId] = null;
                    }
                }
                catch (Exception)
                {
                    userResults[userId] = null;
                }
            }

            var data = reviewsOnPageTemp
                .Select(tempReview =>
                {
                    userResults.TryGetValue(tempReview.UserId, out var userInfo);

                    return new GetReviewDTO(
                        tempReview.Id,
                        tempReview.RecipeId,
                        new CommonUserDataDTO(
                            tempReview.UserId,
                            userInfo?.Username,
                            userInfo?.PhotoUrl
                        ),
                        (float)tempReview.Rating,
                        tempReview.Description,
                        tempReview.HasPhotos,
                        tempReview.PhotoUrls
                    );
                })
                .ToList();

            var totalPages = 0;
            if (totalElements > 0 && pageSize > 0)
            {
                totalPages = (int)Math.Ceiling(totalElements / (double)pageSize);
            }
            if (totalPages == 0 && totalElements > 0) totalPages = 1;

            var paginatedResult = new PaginatedResult<IEnumerable<GetReviewDTO>>
            {
                Data = data,
                TotalElements = totalElements,
                TotalPages = totalPages,
                Page = pageNumber,
                PageSize = pageSize
            };

            return paginatedResult;
        }

        public async Task<Guid> AddReview(AddReviewRequestDTO dto, Guid userId, Guid recipeId, CancellationToken ct)
        {
            var review = new Review
            {
                RecipeId = recipeId,
                UserId = userId,
                Rating = (int)Math.Round(dto.Rating),
                Description = dto.Description,
                HasPhotos = (dto.Photos != null && dto.Photos.Any())
            };

            _context.Reviews.Add(review);

            if (dto.Photos != null && dto.Photos.Any())
            {
                foreach (var photoDto in dto.Photos)
                {
                    var reviewPhoto = new ReviewPhoto
                    {
                        Review = review,
                        PhotoId = photoDto.PhotoId
                    };
                    _context.ReviewPhotos.Add(reviewPhoto);
                }
            }

            await _context.SaveChangesAsync(ct);
            return review.Id;
        }

        public async Task DeleteReview(Guid recipeId, Guid userId, CancellationToken ct)
        {
            var review = await _context.Reviews
                .Include(r => r.ReviewPhotos)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);

            if (review != null)
            {
                _context.ReviewPhotos.RemoveRange(review.ReviewPhotos);
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync(ct);
            }
        }

    }
}
