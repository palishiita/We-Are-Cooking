namespace RecipesAPI.Model.Reviews.Get
{
    public class GetReviewDTO
    {
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public float Rating { get; set; }
        public string? Description { get; set; }
        public bool HasPhotos { get; set; }
        public IEnumerable<string>? PhotoUrls { get; set; }
    }
}
