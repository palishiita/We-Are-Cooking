namespace RecipesAPI.Model.UserData.Restrictions.Get
{
    public record GetRestrictionsWithTagIdsIngredientIdsDTO(Guid Id, Guid UserId, Guid[] IngredientCategoryIds, Guid[] IngredientIds);
}
