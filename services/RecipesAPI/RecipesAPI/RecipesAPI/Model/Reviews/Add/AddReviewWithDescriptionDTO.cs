namespace RecipesAPI.Model.Reviews.Add
{
    public record AddReviewWithDescriptionDTO(
        float Rating, 
        string Description, 
        bool HasPhotos
        );
}
