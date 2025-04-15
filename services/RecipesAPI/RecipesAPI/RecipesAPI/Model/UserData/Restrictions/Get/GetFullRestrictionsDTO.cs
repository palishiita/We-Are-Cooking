using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Model.UserData.Restrictions.Get
{
    public record GetFullRestrictionsDTO(Guid Id, Guid UserId, GetIngredientTagDTO[] IngredientCategories, GetIngredientDTO[] RestrictedIngredients);
}
