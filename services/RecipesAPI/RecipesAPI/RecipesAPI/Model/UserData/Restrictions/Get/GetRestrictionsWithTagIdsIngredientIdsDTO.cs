namespace RecipesAPI.Model.UserData.Restrictions.Get
{
    public record GetRestrictionsWithTagIdsIngredientIdsDTO(Guid Id, Guid UserId, IEnumerable<Guid> IngredientCategoryIds, IEnumerable<Guid> IngredientIds);
}
