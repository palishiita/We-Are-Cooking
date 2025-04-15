namespace RecipesAPI.Model.UserData.Cookbook
{
    public record GetCookbookWithRecipeIdsDTO(Guid Id, Guid UserId, Guid[] RecipeIds);
}
