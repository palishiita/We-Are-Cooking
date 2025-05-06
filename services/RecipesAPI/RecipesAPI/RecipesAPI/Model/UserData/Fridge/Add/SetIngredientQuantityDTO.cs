namespace RecipesAPI.Model.UserData.Fridge.Add
{
    public record SetIngredientQuantityDTO(Guid IngredientId, double Quantity, Guid UnitId);
}
