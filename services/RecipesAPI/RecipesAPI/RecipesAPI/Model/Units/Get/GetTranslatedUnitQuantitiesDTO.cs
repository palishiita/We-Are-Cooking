namespace RecipesAPI.Model.Units.Get
{
    public record GetTranslatedUnitQuantitiesDTO(Guid IngredientId, string IngredientName, Guid UnitOneId, string UnitOneName, Guid UnitTwoId, string UnitTwoName, double StartQuantity, double TranslatedQuantity);
}
