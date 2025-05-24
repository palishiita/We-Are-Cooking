namespace RecipesAPI.Model.Reviews.Get
{
    public record GetReviewDTO(
        Guid Id,
        Guid RecipeId,
        Guid UserId,
        float Rating,
        string? Description,
        bool HasPhotos,
        IEnumerable<string>? PhotoUrls
        );
}
