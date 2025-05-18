using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Units.Get;
using RecipesAPI.Model.Units.Request;

namespace RecipesAPI.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<GetIngredientDTO> GetIngredientById(Guid ingredientId, CancellationToken ct);
        Task<GetIngredientWithCategoriesDTO> GetIngredientWithCategoriesById(Guid ingredientId, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetIngredientDTO>>> GetAllIngredients(int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetIngredientWithCategoriesDTO>>> GetAllIngredientsWithCategories(int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);

        Task<IEnumerable<GetIngredientCategoryDTO>> GetIngredientCategories(Guid ingredientId, CancellationToken ct);

        Task<PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>> GetAllIngredientCategories(int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);

        Task<Guid> AddIngredient(AddIngredientDTO ingredientDTO);
        Task<Guid> AddIngredientWithCategoriesByNames(AddIngredientWithCategoryNamesDTO ingredientDTO);
        Task<Guid> AddIngredientWithCategoriesByIds(AddIngredientWithCategoryIdsDTO ingredientDTO);

        Task<Guid> AddIngredientCategory(AddIngredientCategoryDTO ingredientDTO);

        Task<GetUnitDTO> GetUnitById(Guid unitId, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetUnitDTO>>> GetAllUnits(int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);

        Task<GetTranslatedUnitQuantitiesDTO> GetTranslatedUnitQuantities(RequestUnitQuantityTranslationDTO dto, CancellationToken ct);
    }
}
