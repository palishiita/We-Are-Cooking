namespace RecipesAPI.Model.UserData.Fridge.Get
{
    public record GetFridgeIngredientDataDTO(Guid IngredientId, string Name, string Description, double Quantity, Guid UnitId, string UnitName);
}
