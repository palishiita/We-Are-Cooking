using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IIngredientService
    {
        GetIngredientDTO GetIngredientById(Guid ingredientId);
        GetIngredientWithCategoriesDTO GetIngredientWithCategoriesById(Guid ingredientId);
        Task<PaginatedResult<IEnumerable<GetIngredientDTO>>> GetAllIngredients(int count, int page, bool orderByAsc, string sortBy, string query);
        Task<PaginatedResult<IEnumerable<GetIngredientWithCategoriesDTO>>> GetAllIngredientsWithCategories(int count, int page, bool orderByAsc, string sortBy, string query);

        IEnumerable<GetIngredientCategoryDTO> GetIngredientCategories(Guid ingredientId);

        Task<PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>> GetAllIngredientCategories(int count, int page, bool orderByAsc, string sortBy, string query);

        Task<Guid> AddIngredient(AddIngredientDTO ingredientDTO);
        Task<Guid> AddIngredientWithCategoriesByNames(AddIngredientWithCategoryNamesDTO ingredientDTO);
        Task<Guid> AddIngredientWithCategoriesByIds(AddIngredientWithCategoryIdsDTO ingredientDTO);

        Task<Guid> AddIngredientCategory(AddIngredientCategoryDTO ingredientDTO);
    }
}
