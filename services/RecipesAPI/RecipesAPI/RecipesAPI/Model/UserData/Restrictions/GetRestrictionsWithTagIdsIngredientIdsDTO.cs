namespace RecipesAPI.Model.UserData.Restrictions
{
    public record GetRestrictionsWithTagIdsIngredientIdsDTO(Guid Id, Guid UserId, Guid[] IngredientCategoryIds, Guid[] IngredientIds);
}
