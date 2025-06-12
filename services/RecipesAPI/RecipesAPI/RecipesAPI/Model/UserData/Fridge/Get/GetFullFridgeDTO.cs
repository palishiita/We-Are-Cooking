using RecipesAPI.Model.Common;

namespace RecipesAPI.Model.UserData.Fridge.Get
{
    public record GetFullFridgeDTO(Guid UserId, PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>> Ingredients);
}
