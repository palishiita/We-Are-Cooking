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
using RecipesAPI.Model.Units.Request;
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
        private readonly DbSet<UserDietaryRestriction> _dietaryRestrictions;
        private readonly DbSet<IngredientCategory> _ingredientCategories;

        private readonly HashSet<string> _recipeProps;
        private readonly HashSet<string> _ingredientProps;
        private readonly HashSet<string> _categoryProps;

        private readonly IIngredientService _ingredientService;

        public UserDataService(ILogger<UserDataService> logger, RecipeDbContext dbContext, IIngredientService ingredientService)
        {
            _logger = logger;

            _dbContext = dbContext;
            _cookbookRecipes = _dbContext.Set<UserCookbookRecipe>();
            _recipes = _dbContext.Set<Recipe>();
            _fridgeIngredients = _dbContext.Set<UserFridgeIngredient>();
            _ingredients = _dbContext.Set<Ingredient>();
            _recipeIngredients = _dbContext.Set<RecipeIngredient>();
            _units = _dbContext.Set<Unit>();
            _dietaryRestrictions = _dbContext.Set<UserDietaryRestriction>();
            _ingredientCategories = _dbContext.Set<IngredientCategory>();

            _recipeProps = typeof(Recipe)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _ingredientProps = typeof(Ingredient)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _categoryProps = typeof(IngredientCategory)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _ingredientService = ingredientService;
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
            recipes = recipes
                .Where(cr => cr.UserId == userId)
                .Include(x => x.Recipe);
                
            if (!string.IsNullOrEmpty(query))
            {
                recipes = recipes.Where(cr => cr.Recipe.Name.Contains(query));
            }

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
                        cr.Recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User")))
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
        public async Task RemoveUsedIngredientsInRecipe(Guid userId, Guid recipeId)
        {
            var recipeIngredients = _recipeIngredients
                .Where(x => x.RecipeId == recipeId)
                .Include(x => x.Ingredient)
                .Include(x => x.Unit);

            // does not matter if enough
            var fridgeIngredientsForUse = await _fridgeIngredients
                .Where(x => x.UserId == userId)
                .Where(x => recipeIngredients.Any(y => y.IngredientId == x.IngredientId))
                .Include(x => x.Unit)
                .ToListAsync();

            foreach (var ingredient in fridgeIngredientsForUse)
            {
                var recipeIngredient = recipeIngredients
                    .Where(x => x.IngredientId == ingredient.IngredientId)
                    .FirstOrDefault();

                var quantity = 0.0;

                if (recipeIngredient!.UnitId == ingredient.UnitId)
                {
                    quantity = ingredient.IngredientQuantity - recipeIngredient!.Quantity;
                }
                else
                {
                    try
                    {
                        quantity = ingredient.IngredientQuantity - _ingredientService.GetTranslatedUnitQuantities(
                            new RequestUnitQuantityTranslationDTO(
                                recipeIngredient.IngredientId,
                                recipeIngredient.UnitId,
                                ingredient.UnitId,
                                recipeIngredient.Quantity))
                            .TranslatedQuantity;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        quantity = ingredient.IngredientQuantity;
                    }
                }

                ingredient.IngredientQuantity = double.Max(0.0, Math.Round(quantity, 3));
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var fridgeIngredientsToDelete = fridgeIngredientsForUse.Where(x => Math.Round(x.IngredientQuantity, 3) <= 0.0);
                var fridgeIngredientsToUpdate = fridgeIngredientsForUse.Where(x => !fridgeIngredientsToDelete.Contains(x));

                _fridgeIngredients.RemoveRange(fridgeIngredientsToDelete);
                _fridgeIngredients.UpdateRange(fridgeIngredientsToUpdate);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveUsedIngredientsInRecipe)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>> GetFridgeIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query)
        {
            // query
            IQueryable<UserFridgeIngredient> fridgeIngredients = _fridgeIngredients
                .Where(x => x.UserId == userId)
                .Include(x => x.Ingredient);

            if (!string.IsNullOrEmpty(query))
            {
                fridgeIngredients = fridgeIngredients.Where(x => x.Ingredient.Name.Contains(query));
            }

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
                .Select(x => x.IngredientId);

            var restrictedCategoriesIds = _dietaryRestrictions
                .Where(x => x.UserId == userId)
                .Select(x => x.IngredientCategoryId);

            // query
            var recipes = _recipeIngredients
                .Include(x => x.Recipe)
                    .ThenInclude(x => x.Ingredients)
                        .ThenInclude(x => x.Ingredient.Connections)
                .Include(x => x.Recipe)
                    .ThenInclude(x => x.Ingredients)
                        .ThenInclude(x => x.Unit)
                .Where(x => !x.Recipe.Ingredients.Any(y => restrictedCategoriesIds.Contains(y.IngredientId)))
                .Where(x => x.Recipe.Ingredients
                    .All(y => fridgeIngredientIds.Contains(y.IngredientId)));

            if (!string.IsNullOrEmpty(query))
            {
                recipes = recipes.Where(x => x.Recipe.Name.Contains(query));
            }

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
                    recipeIngredient.Recipe.Ingredients.Select(y => new GetRecipeIngredientDTO(
                        y.IngredientId,
                        y.Ingredient.Name,
                        y.Ingredient.Description ?? "",
                        y.Quantity,
                        y.UnitId,
                        y.Unit.Name)),
                    new CommonUserDataDTO(
                        recipeIngredient.Recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User"
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

        #region Restrictions

        public async Task AddUserRestrictedCategories(Guid userId, IEnumerable<Guid> categoryIds)
        {
            var categories = _ingredientCategories.Where(x => categoryIds.Contains(x.Id));

            var restrictions = new List<UserDietaryRestriction>();

            foreach (var category in categories) 
            {
                var restriction = new UserDietaryRestriction
                {
                    UserId = userId,
                    IngredientCategoryId = category.Id,
                };
                restrictions.Add(restriction);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dietaryRestrictions.AddRangeAsync(restrictions);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddUserRestrictedCategories)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveUserRestrictedCategories(Guid userId, IEnumerable<Guid> categoryIds)
        {
            var restrictions = _dietaryRestrictions.Where(x => categoryIds.Contains(x.IngredientCategoryId));

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _dietaryRestrictions.RemoveRange(restrictions);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddUserRestrictedCategories)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>> GetUserRestrictedCategories(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query)
        {
            // query
            IQueryable<UserDietaryRestriction> categories = _dietaryRestrictions
                .Where(x => x.UserId == userId)
                .Include(x => x.IngredientCategory);

            if (!string.IsNullOrEmpty(query))
            {
                categories = categories.Where(x => x.IngredientCategory.Name.Contains(query));
            }

            // order
            if (_categoryProps.Contains(sortBy))
            {
                categories = categories.OrderByChildProperties("IngredientCategory", sortBy, orderByAsc);
            }
            else
            {
                categories = orderByAsc ? categories.OrderBy(x => x.IngredientCategory.Name) : categories.OrderByDescending(x => x.IngredientCategory.Name);
            }

            // count
            int totalCount = await categories.CountAsync();

            // project
            var data = await categories
                .Select(x => new GetIngredientCategoryDTO(
                    x.IngredientCategoryId,
                    x.IngredientCategory.Name,
                    x.IngredientCategory.Description))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task<PaginatedResult<IEnumerable<GetIngredientDTO>>> GetUserRestrictedIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query)
        {
            var restrictedCategoriesIds = _dietaryRestrictions
                .Where(x => x.UserId == userId)
                .Include(x => x.IngredientCategory)
                .Select(x => x.IngredientCategoryId);

            // query
            var ingredients = _ingredients
                .Include(x => x.Connections)
                .ThenInclude(x => x.IngredientCategory)
                .Where(x => !x.Connections.Any(y => restrictedCategoriesIds.Contains(y.IngredientCategory.Id)));

            if (!string.IsNullOrEmpty(query))
            {
                ingredients = ingredients.Where(x => x.Name.Contains(query));
            }

            // order
            if (_ingredientProps.Contains(sortBy))
            {
                ingredients = ingredients.OrderByChildProperties("IngredientCategory", sortBy, orderByAsc);
            }
            else
            {
                ingredients = orderByAsc ? ingredients.OrderBy(x => x.Name) : ingredients.OrderByDescending(x => x.Name);
            }

            // count
            int totalCount = await ingredients.CountAsync();

            // project
            var data = await ingredients
                .Select(x => new GetIngredientDTO(
                    x.Id,
                    x.Name,
                    x.Description ?? ""))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetIngredientDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        #endregion Restrictions
    }
}
