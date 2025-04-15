namespace RecipesAPI.Model.Ingredients
{
    public record GetFullIngredientDataDTO(Guid Id, GetIngredientTagDTO[] IngredientTags);
}
