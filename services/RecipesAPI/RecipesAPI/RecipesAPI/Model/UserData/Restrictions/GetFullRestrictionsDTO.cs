using RecipesAPI.Model.Ingredients;

namespace RecipesAPI.Model.UserData.Restrictions
{
    public record GetFullRestrictionsDTO(Guid Id, Guid UserId, GetIngredientTagDTO[] IngredientCategories, GetIngredientDTO[] RestrictedIngredients);
}
