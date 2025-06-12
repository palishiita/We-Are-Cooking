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
        private readonly IUserInfoService _userInfoService;

        public UserDataService(ILogger<UserDataService> logger, RecipeDbContext dbContext, IIngredientService ingredientService, IUserInfoService userInfoService)
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
            _userInfoService = userInfoService;
        }

        #region Cookbook
        public async Task AddRecipeToCookbook(Guid userId, AddRecipeToCookbookDTO recipeDTO, CancellationToken ct)
        {
            var cookbookRecipe = new UserCookbookRecipe()
            {
                UserId = userId,
                RecipeId = recipeDTO.RecipeId,
                IsFavorite = recipeDTO.SetAsFavorite
            };

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                _cookbookRecipes.Add(cookbookRecipe);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddRecipeToCookbook)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }

        }

        public IEnumerable<Guid> GetIdsOfPresentRecipesInCookbook(Guid userId, IEnumerable<Guid> recipeIds)
        {
            return _cookbookRecipes
                .Where(x => x.UserId == userId)
                .Where(x => recipeIds.Contains(x.RecipeId))
                .Select(x => x.RecipeId);
        }

        public async Task ChangeRecipeFavoriteStatus(Guid userId, ChangeRecipeFavoriteStatusDTO statusDTO, CancellationToken ct)
        {
            var cookbookRecipe = _cookbookRecipes
            .Where(cr => cr.UserId == userId)
            .FirstOrDefault(x => x.RecipeId == statusDTO.RecipeId) ?? throw new CookbookRecipeNotFound($"Recipe with id {statusDTO.RecipeId} is not connected to user with id {userId}.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                cookbookRecipe.IsFavorite = statusDTO.IsFavorite;
                _cookbookRecipes.Update(cookbookRecipe);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(ChangeRecipeFavoriteStatus)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>> GetFullUserCookbook(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, bool showOnlyFavorites, CancellationToken ct)
        {
            var recipes = showOnlyFavorites ? _cookbookRecipes.Where(cr => cr.IsFavorite) : _cookbookRecipes;

            // query
            recipes = recipes
                .Where(cr => cr.UserId == userId)
                .Include(x => x.Recipe);
                
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                recipes = recipes.Where(cr => cr.Recipe.Name.ToUpper().Contains(query));
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
            int totalCount = await recipes.CountAsync(ct);

            // project
            var recipesFromCookbook = await recipes
                .Include(cr => cr.Recipe)
                    .ThenInclude(r => r.Ingredients)
                        .ThenInclude(ri => ri.Ingredient)
                .Include(cr => cr.Recipe)
                    .ThenInclude(r => r.Ingredients)
                        .ThenInclude(ri => ri.Unit)
                .Select(cr => new
                {
                    cr.RecipeId,
                    cr.Recipe.Name,
                    cr.Recipe.Description,
                    Ingredients = cr.Recipe.Ingredients.Select(i => new GetRecipeIngredientDTO(
                        i.IngredientId,
                        i.Ingredient.Name,
                        i.Ingredient.Description ?? "",
                        i.Quantity,
                        i.UnitId,
                        i.Unit.Name)),
                    cr.IsFavorite,
                    cr.Recipe.PostingUserId
                })
                .ToListAsync(ct);

            var userIds = recipesFromCookbook.Select(r => r.PostingUserId).Distinct();

            // fetch all user info in parallel (or use a batch API if available)
            var userInfoTasks = userIds
                .ToDictionary(
                    id => id,
                    id => _userInfoService.GetUserById(id, userId)
                );

            await Task.WhenAll(userInfoTasks.Values);

            var data = recipesFromCookbook
                .Select(recipe => new GetFullRecipeForCookbookDTO(
                    recipe.RecipeId,
                    recipe.Name,
                    recipe.Description,
                    recipe.Ingredients,
                    recipe.IsFavorite,
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        userInfoTasks[recipe.PostingUserId].Result.Username,
                        userInfoTasks[recipe.PostingUserId].Result.ImageUrl
            ))).ToList();

            return new PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task RemoveRecipesFromCookbook(Guid userId, IEnumerable<Guid> recipeIds, CancellationToken ct)
        {
            var cookbookRecipes = _cookbookRecipes
                .Where(x => x.UserId == userId)
                .Where(x => recipeIds.Contains(x.RecipeId));

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                _cookbookRecipes.RemoveRange(cookbookRecipes);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveRecipesFromCookbook)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        #endregion Cookbook

        #region Fridge

        public async Task SetFridgeIngredients(Guid userId, IEnumerable<SetIngredientQuantityDTO> ingredientsData, CancellationToken ct)
        {
            var previous = _fridgeIngredients.Where(x => x.UserId == userId);

            var next = ingredientsData.Select(id => new UserFridgeIngredient()
            {
                UserId = userId,
                IngredientId = id.IngredientId,
                UnitId = id.UnitId,
                IngredientQuantity = id.Quantity
            }).ToList();

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                _fridgeIngredients.RemoveRange(previous);
                await _dbContext.SaveChangesAsync(ct);

                await _fridgeIngredients.AddRangeAsync(next, ct);
                await _dbContext.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(SetFridgeIngredients)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        // this will be done later, when the Units are applied to the recipe ingredients as well
        public async Task RemoveUsedIngredientsInRecipe(Guid userId, Guid recipeId, CancellationToken ct)
        {
            var recipeIngredients = await _recipeIngredients
                .Where(x => x.RecipeId == recipeId)
                .Include(x => x.Ingredient)
                .Include(x => x.Unit)
                .ToListAsync(ct);

            // does not matter if enough
            var fridgeIngredientsForUse = await _fridgeIngredients
                .Where(x => x.UserId == userId)
                .Where(x => recipeIngredients.Any(y => y.IngredientId == x.IngredientId))
                .Include(x => x.Unit)
                .ToListAsync(ct);

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
                        quantity = ingredient.IngredientQuantity - (await _ingredientService.GetTranslatedUnitQuantities(
                            new RequestUnitQuantityTranslationDTO(
                                recipeIngredient.IngredientId,
                                recipeIngredient.UnitId,
                                ingredient.UnitId,
                                recipeIngredient.Quantity), ct)).TranslatedQuantity;
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogInformation(ex, ex.Message);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        quantity = ingredient.IngredientQuantity;
                    }
                }

                ingredient.IngredientQuantity = double.Max(0.0, Math.Round(quantity, 3));
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var fridgeIngredientsToDelete = fridgeIngredientsForUse.Where(x => Math.Round(x.IngredientQuantity, 3) <= 0.0);
                var fridgeIngredientsToUpdate = fridgeIngredientsForUse.Where(x => !fridgeIngredientsToDelete.Contains(x));

                _fridgeIngredients.RemoveRange(fridgeIngredientsToDelete);
                _fridgeIngredients.UpdateRange(fridgeIngredientsToUpdate);

                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveUsedIngredientsInRecipe)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>> GetFridgeIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct)
        {
            // query
            IQueryable<UserFridgeIngredient> fridgeIngredients = _fridgeIngredients
                .Where(x => x.UserId == userId)
                .Include(x => x.Ingredient);

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                fridgeIngredients = fridgeIngredients.Where(x => x.Ingredient.Name.ToUpper().Contains(query));
            }

            // order
            if (_ingredientProps.Contains(sortBy))
            {
                fridgeIngredients = fridgeIngredients.OrderByChildProperties("Ingredient", sortBy, orderByAsc);
            }
            else
            {
                fridgeIngredients = fridgeIngredients.OrderBy(x => x.Ingredient.Name);
            }

            // count
            int totalCount = await fridgeIngredients.CountAsync(ct);

            var data = await fridgeIngredients
                .Include(x => x.Ingredient)
                .Include(x => x.Unit)
                .Skip((page - 1) * count)
                .Take(count)
                .Select(x => new GetFridgeIngredientDataDTO(
                    x.IngredientId, 
                    x.Ingredient.Name, 
                    x.Ingredient.Description ?? "", 
                    x.IngredientQuantity, 
                    x.UnitId, 
                    x.Unit.Name))
                .ToListAsync(ct);

            return new PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>()
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetRecipesAvailableWithFridge(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct)
        {
            var fridgeIngredientIds = _fridgeIngredients
                .Where(fridgeIngredient => fridgeIngredient.UserId == userId)
                .Select(fridgeIngredient => fridgeIngredient.IngredientId);

            var restrictedCategoriesIds = _dietaryRestrictions
                .Where(restrictedCategory => restrictedCategory.UserId == userId)
                .Select(restrictedCategory => restrictedCategory.IngredientCategoryId);

            // query
            var recipes = _recipes
                .Include(r => r.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
                        .ThenInclude(i => i.Connections)
                .Include(r => r.Ingredients)
                    .ThenInclude(ri => ri.Unit)
                .Where(r => !r.Ingredients.Any(ri => restrictedCategoriesIds.Contains(ri.IngredientId)))
                .Where(r => r.Ingredients.All(ri => fridgeIngredientIds.Contains(ri.IngredientId)));


            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                recipes = recipes.Where(recipe => recipe.Name.ToUpper().Contains(query));
            }

            // order
            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(recipe => recipe.Name) : recipes.OrderByDescending(recipe => recipe.Name);
            }

            // count
            int totalCount = await recipes.CountAsync(ct);

            // project
            var recipeDTOs = await recipes
                .Select(recipe => new
                {
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    Ingredients = recipe.Ingredients.Select(ingredient => new GetRecipeIngredientDTO(
                        ingredient.IngredientId,
                        ingredient.Ingredient.Name,
                        ingredient.Ingredient.Description ?? "",
                        ingredient.Quantity,
                        ingredient.UnitId,
                        ingredient.Unit.Name)),
                    recipe.PostingUserId
                })
                .ToListAsync(ct);

            var userIds = recipeDTOs.Select(r => r.PostingUserId).Distinct();

            // fetch all user info in parallel (or use a batch API if available)
            var userInfoTasks = userIds
                .ToDictionary(
                    id => id,
                    id => _userInfoService.GetUserById(id, userId)
                );

            await Task.WhenAll(userInfoTasks.Values);

            var data = recipeDTOs
                .Select(recipe => new GetFullRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    recipe.Ingredients,
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        userInfoTasks[recipe.PostingUserId].Result.Username,
                        userInfoTasks[recipe.PostingUserId].Result.ImageUrl
            ))).ToList();

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

        public async Task AddUserRestrictedCategories(Guid userId, IEnumerable<Guid> categoryIds, CancellationToken ct)
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

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                await _dietaryRestrictions.AddRangeAsync(restrictions);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddUserRestrictedCategories)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task RemoveUserRestrictedCategories(Guid userId, IEnumerable<Guid> categoryIds, CancellationToken ct)
        {
            var restrictions = _dietaryRestrictions.Where(x => categoryIds.Contains(x.IngredientCategoryId) && x.UserId == userId);

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                _dietaryRestrictions.RemoveRange(restrictions);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddUserRestrictedCategories)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>> GetUserRestrictedCategories(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct)
        {
            // query
            IQueryable<UserDietaryRestriction> categories = _dietaryRestrictions
                .Where(x => x.UserId == userId)
                .Include(x => x.IngredientCategory);

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                categories = categories.Where(x => x.IngredientCategory.Name.ToUpper().Contains(query));
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
            int totalCount = await categories.CountAsync(ct);

            // project
            var data = await categories
                .Select(x => new GetIngredientCategoryDTO(
                    x.IngredientCategoryId,
                    x.IngredientCategory.Name,
                    x.IngredientCategory.Description))
                .ToListAsync(ct);

            return new PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task<PaginatedResult<IEnumerable<GetIngredientDTO>>> GetUserRestrictedIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct)
        {
            var restrictedCategoriesIds = _dietaryRestrictions
                .Where(x => x.UserId == userId)
                .Include(x => x.IngredientCategory)
                .Select(x => x.IngredientCategoryId);

            var ingredients = _ingredients
                .Include(x => x.Connections)
                .ThenInclude(x => x.IngredientCategory)
                .Where(x => !x.Connections.Any(y => restrictedCategoriesIds.Contains(y.IngredientCategory.Id)));

            // query
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                ingredients = ingredients.Where(x => x.Name.ToUpper().Contains(query));
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
            int totalCount = await ingredients.CountAsync(ct);

            // project
            var data = await ingredients
                .Select(x => new GetIngredientDTO(
                    x.Id,
                    x.Name,
                    x.Description ?? ""))
                .ToListAsync(ct);

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
