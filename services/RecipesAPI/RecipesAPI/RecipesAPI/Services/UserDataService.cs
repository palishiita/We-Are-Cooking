using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Entities.UserData;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Extensions;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
using RecipesAPI.Model.UserData.Cookbook.Update;
using RecipesAPI.Model.UserData.Fridge.Add;
using RecipesAPI.Model.UserData.Fridge.Delete;
using RecipesAPI.Model.UserData.Fridge.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Services
{
    public class UserDataService : IUserDataService
    {
        ILogger<UserDataService> _logger;

        private readonly RecipeDbContext _dbContext;
        private readonly DbSet<UserCookbookRecipe> _cookbookRecipes;
        private readonly DbSet<UserFridgeIngredient> _fridgeIngredients;
        private readonly DbSet<Ingredient> _ingredients;
        private readonly DbSet<Unit> _units;

        private readonly HashSet<string> _recipeProps;
        private readonly HashSet<string> _ingredientProps;

        public UserDataService(ILogger<UserDataService> logger, RecipeDbContext dbContext)
        {
            _logger = logger;

            _dbContext = dbContext;
            _cookbookRecipes = _dbContext.Set<UserCookbookRecipe>();
            _fridgeIngredients = _dbContext.Set<UserFridgeIngredient>();
            _ingredients = _dbContext.Set<Ingredient>();
            _units = _dbContext.Set<Unit>();

            _recipeProps = typeof(Recipe)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _ingredientProps = typeof(Ingredient)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        #region Cookbook
        public async Task AddRecipeToCookbook(Guid userId, AddRecipeToCookbookDTO recipeDTO)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var cookbookRecipe = new UserCookbookRecipe()
            {
                UserId = userId,
                RecipeId = recipeDTO.RecipeId,
                IsFavorite = recipeDTO.SetAsFavorite
            };

            try
            {
                _cookbookRecipes.Add(cookbookRecipe);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddRecipeToCookbook)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task ChangeRecipeFavoriteStatus(Guid userId, ChangeRecipeFavoriteStatusDTO statusDTO)
        {
            var cookbookRecipe = _cookbookRecipes
            .Where(cr => cr.UserId == userId)
            .FirstOrDefault(x => x.RecipeId == statusDTO.RecipeId) ?? throw new CookbookRecipeNotFound($"Recipe with id {statusDTO.RecipeId} is not connected to user with id {userId}.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                cookbookRecipe.IsFavorite = statusDTO.IsFavorite;
                _cookbookRecipes.Update(cookbookRecipe);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(ChangeRecipeFavoriteStatus)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>> GetFullUserCookbook(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, bool showOnlyFavorites)
        {
            var recipes = showOnlyFavorites ? _cookbookRecipes.Where(cr => cr.IsFavorite) : _cookbookRecipes;


            // query
            recipes = string.IsNullOrEmpty(query)
                ? recipes.Where(cr => cr.UserId == userId)
                : recipes
                    .Where(cr => cr.UserId == userId)
                    .Include(cr => cr.Recipe)
                    .Where(cr => cr.Recipe.Name.Contains(query));

            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderByChildProperties("Recipe", sortBy, orderByAsc);
            }
            else if (sortBy.Equals("IsFavorite", StringComparison.InvariantCultureIgnoreCase))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Recipe.Name) : recipes.OrderByDescending(r => r.Recipe.Name);
            }

            // count
            int totalCount = await recipes.CountAsync();

            // project
            var recipesFromCookbook = await recipes
                .Include(cr => cr.Recipe)
                    .ThenInclude(r => r.Ingredients)
                        .ThenInclude(ri => ri.Ingredient)
                .Include(cr => cr.Recipe)
                    .ThenInclude(r => r.PostingUser)
                .Select(cr => new GetFullRecipeForCookbookDTO(
                    cr.RecipeId,
                    cr.Recipe.Name,
                    cr.Recipe.Description,
                    cr.Recipe.Ingredients.Select(i => new GetIngredientDTO(
                        i.IngredientId,
                        i.Ingredient.Name,
                        i.Ingredient.Description ?? "")),
                    cr.IsFavorite,
                    new CommonUserDataDTO(
                        cr.UserId,
                        cr.User.FistName,
                        cr.User.SecondName ?? "",
                        cr.User.LastName)))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>
            {
                Data = recipesFromCookbook,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task RemoveRecipesFromCookbook(Guid userId, IEnumerable<Guid> recipeIds)
        {
            var cookbookRecipes = _cookbookRecipes
                .Where(x => x.UserId == userId)
                .Where(x => recipeIds.Contains(x.RecipeId));

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _cookbookRecipes.RemoveRange(cookbookRecipes);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveRecipesFromCookbook)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion Cookbook

        #region Fridge

        public Task UpdateFridgeIngredients(Guid userId, IEnumerable<SetIngredientQuantityDTO> ingredientsData)
        {
            throw new NotImplementedException();
        }

        public Task RemoveIngredientsFromFridge(Guid userId, IEnumerable<RemoveIngredientQuantityDTO> ingredientsData)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUsedIngredientsInRecipe(Guid userId, Guid recipeId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<GetFridgeIngredientDataDTO>> GetFridgeIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<GetFullRecipeDTO>> GetRecipesAvailableWithFridge(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query)
        {
            throw new NotImplementedException();
        }

        #endregion Fridge
    }
}
