using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Reviews;
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

        public async Task<IEnumerable<GetReviewDTO>> GetReviewsByRecipeId(Guid recipeId)
        {
            return await _context.Reviews
                .Where(r => r.RecipeId == recipeId)
                .Select(r => new GetReviewDTO
                {
                    Id = r.Id,
                    RecipeId = r.RecipeId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Description = r.Description,
                    HasPhotos = r.HasPhotos,
                    PhotoUrls = r.ReviewPhotos.Select(p => p.PhotoUrl.Url)
                })
                .ToListAsync();
        }

        public async Task<Guid> AddReviewWithDescription(AddReviewWithDescriptionDTO dto, Guid userId, Guid recipeId)
        {
            var review = new Review
            {
                RecipeId = recipeId,
                UserId = userId,
                Rating = dto.Rating,
                Description = dto.Description,
                HasPhotos = dto.HasPhotos
            };

            _context.Reviews.Add(review); 
            await _context.SaveChangesAsync();
            return review.Id;
        }

        public async Task<Guid> AddReviewWithPhotos(AddReviewWithPhotosDTO dto, Guid userId, Guid recipeId)
        {
            var review = new Review
            {
                RecipeId = recipeId,
                UserId = userId,
                Rating = dto.Rating,
                HasPhotos = true
            };
            _context.Reviews.Add(review);

            foreach (var photoDto in dto.Photos)
            {
                var reviewPhoto = new ReviewPhoto
                {
                    Review = review,
                    PhotoId = photoDto.PhotoId
                };
                _context.ReviewPhotos.Add(reviewPhoto);
            }
            await _context.SaveChangesAsync();
            return review.Id;
        }

        public async Task DeleteReview(Guid recipeId, Guid userId)
        {
            var review = await _context.Reviews
                .Include(r => r.ReviewPhotos)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);

            if (review != null)
            {
                _context.ReviewPhotos.RemoveRange(review.ReviewPhotos);
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}
