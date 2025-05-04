namespace RecipesAPI.Model.UserData.Fridge.Get
{
    public record GetFrigeWithIngredientIdsRecipesIdsDTO(Guid Id, Guid UserId, IEnumerable<Guid> IngredientIds, IEnumerable<Guid> RecipeIds);
}
