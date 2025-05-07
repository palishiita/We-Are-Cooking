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
using RecipesAPI.Model.UserData.Fridge.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Services
{
    public class UserDataService : IUserDataService
    {
        ILogger<UserDataService> _logger;

        private readonly RecipeDbContext _dbContext;
        private readonly DbSet<UserCookbookRecipe> _cookbookRecipes;
        private readonly DbSet<Recipe> _recipes;
        private readonly DbSet<UserFridgeIngredient> _fridgeIngredients;
        private readonly DbSet<Ingredient> _ingredients;
        private readonly DbSet<RecipeIngredient> _recipeIngredients;
        private readonly DbSet<Unit> _units;

        private readonly HashSet<string> _recipeProps;
        private readonly HashSet<string> _ingredientProps;

        public UserDataService(ILogger<UserDataService> logger, RecipeDbContext dbContext)
        {
            _logger = logger;

            _dbContext = dbContext;
            _cookbookRecipes = _dbContext.Set<UserCookbookRecipe>();
            _recipes = _dbContext.Set<Recipe>();
            _fridgeIngredients = _dbContext.Set<UserFridgeIngredient>();
            _ingredients = _dbContext.Set<Ingredient>();
            _recipeIngredients = _dbContext.Set<RecipeIngredient>();
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

        public async Task SetFridgeIngredients(Guid userId, IEnumerable<SetIngredientQuantityDTO> ingredientsData)
        {
            var previous = _fridgeIngredients.Where(x => x.UserId == userId);

            if (!_dbContext.Users.Any(x => x.Id == userId))
            {
                throw new UserNotFoundException($"User with id {userId}");
            }

            var next = new List<UserFridgeIngredient>();

            foreach (var ing in ingredientsData)
            {
                var ingredient = new UserFridgeIngredient()
                {
                    UserId = userId,
                    IngredientId = ing.IngredientId,
                    UnitId = ing.UnitId,
                    IngredientQuantity = ing.Quantity,
                };
                next.Add(ingredient);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _fridgeIngredients.RemoveRange(previous);
                await _dbContext.SaveChangesAsync();

                await _fridgeIngredients.AddRangeAsync(next);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(SetFridgeIngredients)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        // this will be done later, when the Units are applied to the recipe ingredients as well
        public Task RemoveUsedIngredientsInRecipe(Guid userId, Guid recipeId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>> GetFridgeIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query)
        {
            // query
            var fridgeIngredients = _fridgeIngredients
                .Where(x => x.UserId == userId)
                .Include(x => x.Ingredient)
                .Where(x => x.Ingredient.Name.Contains(query));

            // sort
            if (_ingredientProps.Contains(sortBy))
            {
                fridgeIngredients = fridgeIngredients.OrderByChildProperties("Ingredient", sortBy, orderByAsc);
            }
            else
            {
                fridgeIngredients = fridgeIngredients.OrderBy(x => x.Ingredient.Name);
            }

            // count
            int totalCount = await _fridgeIngredients.CountAsync();

            var data = await fridgeIngredients
                .Include(x => x.Ingredient)
                .Include(x => x.Unit)
                .Skip(page * count)
                .Take(count)
                .Select(x => new GetFridgeIngredientDataDTO(
                    x.IngredientId, 
                    x.Ingredient.Name, 
                    x.Ingredient.Description ?? "", 
                    x.IngredientQuantity, 
                    x.UnitId, 
                    x.Unit.Name))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>()
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetRecipesAvailableWithFridge(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query)
        {
            var fridgeIngredientIds = _fridgeIngredients
                .Where(x => x.UserId == userId)
                .Select(x => x.UserId);

            // query
            var recipes = _recipeIngredients
                .Include(x => x.Recipe)
                .ThenInclude(x => x.Ingredients)
                .Include(x => x.Recipe)
                .ThenInclude(x => x.PostingUser)
                .Where(x => x.Recipe.Ingredients.All(x => fridgeIngredientIds.Contains(x.IngredientId)))
                .Where(x => x.Recipe.Name.Contains(query));

            // order
            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderByChildProperties("Recipe", sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(x => x.Recipe.Name) : recipes.OrderByDescending(x => x.Recipe.Name);
            }

            // count
            int totalCount = await recipes.CountAsync();

            // project
            var data = await recipes
                .Select(recipeIngredient => new GetFullRecipeDTO(
                    recipeIngredient.RecipeId, 
                    recipeIngredient.Recipe.Name, 
                    recipeIngredient.Recipe.Name, 
                    recipeIngredient.Recipe.Ingredients.Select(y => new GetIngredientDTO(
                        y.IngredientId,
                        y.Ingredient.Name,
                        y.Ingredient.Description ?? "")),
                    new CommonUserDataDTO(
                        userId,
                        recipeIngredient.Recipe.PostingUser.FistName,
                        recipeIngredient.Recipe.PostingUser.SecondName ?? "",
                        recipeIngredient.Recipe.PostingUser.LastName
                        )))
                .ToListAsync();


            return new PaginatedResult<IEnumerable<GetFullRecipeDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        #endregion Fridge
    }
}
