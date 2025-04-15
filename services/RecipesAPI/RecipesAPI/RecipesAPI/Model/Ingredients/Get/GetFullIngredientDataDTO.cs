namespace RecipesAPI.Model.Ingredients.Get
{
    public record GetFullIngredientDataDTO(Guid Id, GetIngredientTagDTO[] IngredientTags);
}
