namespace RecipesAPI.Model.Reviews.Add
{
    public record AddReviewWithPhotosDTO(
        float Rating, 
        bool HasPhotos, 
        IEnumerable<AddPhotosToReviewDTO> Photos
        );
}
