using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Model.Reviews.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<GetReviewDTO>> GetReviewsByRecipeId(Guid recipeId);
        Task AddReviewWithDescription(AddReviewWithDescriptionDTO dto, Guid userId, Guid recipeId);
        Task AddReviewWithPhotos(AddReviewWithPhotosDTO dto, Guid userId, Guid recipeId);
        Task DeleteReview(Guid recipeId, Guid userId);
    }
}
