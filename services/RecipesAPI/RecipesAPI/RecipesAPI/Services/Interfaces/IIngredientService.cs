using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IIngredientService
    {
        GetIngredientDTO GetIngredientById(Guid ingredientId);
        IEnumerable<GetIngredientDTO> GetAllIngredients(int count, int page, bool orderByAsc, string sortBy);

        IEnumerable<GetIngredientCategoryDTO> GetIngredientCategories(Guid ingredientId);

        IEnumerable<GetIngredientCategoryDTO> GetAllIngredientCategories(int count, int page, bool orderByAsc, string sortBy);

        Task<Guid> AddIngredient(AddIngredientDTO ingredientDTO);
        Task<Guid> AddIngredientWithCategoriesByNames(AddIngredientWithCategoryNamesDTO ingredientDTO);
        Task<Guid> AddIngredientWithCategoriesByIds(AddIngredientWithCategoryIdsDTO ingredientDTO);
    }
}
