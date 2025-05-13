namespace RecipesAPI.Model.UserData.Fridge.Get
{
    public record GetFridgeWithIngredientIdsRecipesIdsDTO(Guid UserId, IEnumerable<Guid> IngredientIds, IEnumerable<Guid> RecipeIds);
}
