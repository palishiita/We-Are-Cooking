namespace RecipesAPI.Model.Reviews.Add
{
    public record AddReviewRequestDTO(
        float Rating, 
        string? Description, 
        IEnumerable<AddPhotosToReviewDTO>? Photos
        );
}
