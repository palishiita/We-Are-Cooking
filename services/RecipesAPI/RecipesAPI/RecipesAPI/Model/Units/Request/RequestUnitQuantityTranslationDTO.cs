namespace RecipesAPI.Model.Units.Request
{
    public record RequestUnitQuantityTranslationDTO(Guid IngredientId, Guid UnitOneId, Guid UnitTwoId, double Quantity);
}
