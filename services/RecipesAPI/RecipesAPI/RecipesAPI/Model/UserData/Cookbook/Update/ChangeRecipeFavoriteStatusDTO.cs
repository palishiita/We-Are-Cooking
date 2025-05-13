namespace RecipesAPI.Model.UserData.Cookbook.Update
{
    public record ChangeRecipeFavoriteStatusDTO(Guid UserId, Guid RecipeId, bool IsFavorite);
}
