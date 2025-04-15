namespace RecipesAPI.Model.UserData.Fridge
{
    public record GetFrigeWithIngredientIdsRecipesIdsDTO(Guid Id, Guid UserId, Guid[] IngredientIds, Guid[] RecipeIds);
}
