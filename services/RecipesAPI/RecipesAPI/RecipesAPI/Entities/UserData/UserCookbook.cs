namespace RecipesAPI.Entities.UserData
{
    public class UserCookbook
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public bool IsFavorite { get; set; }
    }
}
