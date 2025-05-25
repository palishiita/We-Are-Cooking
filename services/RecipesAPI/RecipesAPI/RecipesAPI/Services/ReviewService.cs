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

        public ReviewService(RecipeDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<IEnumerable<GetReviewDTO>>> GetReviewsByRecipeId(Guid recipeId, PaginationParameters paginationParameters, CancellationToken ct)
        {
            var query = _context.Reviews
                .Where(r => r.RecipeId == recipeId)
                .Include(r => r.ReviewPhotos)
                    .ThenInclude(revp => revp.PhotoUrl)
                .OrderByDescending(r => r.Id)
                .Select(r => new GetReviewDTO(
                    r.Id,
                    r.RecipeId,
                    r.UserId,
                    (float)r.Rating,
                    r.Description,
                    r.HasPhotos,
                    r.ReviewPhotos.Select(p => p.PhotoUrl.Url).ToList()
                ));

            int pageNumber = paginationParameters.Page;
            int pageSize = paginationParameters.PageSize;

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalElements = await query.CountAsync(ct);
            var itemsOnPage = await query
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync(ct);

            var totalPages = (int)Math.Ceiling(totalElements / (double)pageSize);
            if (totalPages == 0 && totalElements > 0) totalPages = 1;
            if (totalPages == 0 && totalElements == 0) totalPages = 0;

            var paginatedResult = new PaginatedResult<IEnumerable<GetReviewDTO>>
            {
                Data = itemsOnPage,
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
