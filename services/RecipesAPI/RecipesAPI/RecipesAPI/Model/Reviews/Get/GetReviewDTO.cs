using RecipesAPI.Model.Common;

namespace RecipesAPI.Model.Reviews.Get
{
    public record GetReviewDTO(
        Guid Id,
        Guid RecipeId,
        CommonUserDataDTO User,
        float Rating,
        string? Description,
        bool HasPhotos,
        IEnumerable<string>? PhotoUrls
        );
}
