namespace RecipesAPI.Model.UserData.Cookbook.Add
{
    /// <summary>
    /// Add recipe to user's cookbook.
    /// </summary>
    /// <param name="RecipeId"></param>
    /// <param name="SetAsFavorite">Check if the recipe is added as favorite from the start.</param>
    public record AddRecipeToCookbookDTO(Guid RecipeId, bool SetAsFavorite);
}
