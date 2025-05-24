using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Model.Reviews.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<GetReviewDTO>> GetReviewsByRecipeId(Guid recipeId, CancellationToken ct);
        Task<Guid> AddReview(AddReviewRequestDTO dto, Guid userId, Guid recipeId, CancellationToken ct);
        Task DeleteReview(Guid recipeId, Guid userId, CancellationToken ct);
    }
}
