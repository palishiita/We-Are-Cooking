using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Model.UserData.Restrictions.Get
{
    public record GetRestrictedCategoriesDTO(Guid UserId, GetIngredientCategoryDTO[] RestrictedCategories);
}
