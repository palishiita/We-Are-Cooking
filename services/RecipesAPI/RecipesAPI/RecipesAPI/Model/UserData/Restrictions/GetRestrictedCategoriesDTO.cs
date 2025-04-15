using RecipesAPI.Model.Ingredients;

namespace RecipesAPI.Model.UserData.Restrictions
{
    public record GetRestrictedCategoriesDTO(Guid UserId, GetIngredientTagDTO[] RestrictedCategories);
}
