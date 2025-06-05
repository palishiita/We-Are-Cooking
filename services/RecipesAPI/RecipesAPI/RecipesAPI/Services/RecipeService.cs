using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Exceptions.Duplicates;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Extensions;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.Recipes.Update;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Services
{
    public class RecipeService : IRecipeService
    {
        ILogger<RecipeService> _logger;

        private readonly IUserInfoService _userInfoService;

        private readonly RecipeDbContext _dbContext;
        private readonly DbSet<Recipe> _recipes;
        private readonly DbSet<RecipeIngredient> _recipeIngredients;
        private readonly DbSet<Ingredient> _ingredients;

        private readonly HashSet<string> _recipeProps;

        public RecipeService(ILogger<RecipeService> logger, RecipeDbContext dbContext, IUserInfoService userInfoService)
        {
            _logger = logger;

            _dbContext = dbContext;
            _recipeProps = typeof(Recipe)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _recipes = dbContext.Set<Recipe>();
            _ingredients = dbContext.Set<Ingredient>();
            _recipeIngredients = dbContext.Set<RecipeIngredient>();
            _userInfoService = userInfoService;
        }

        public async Task<Guid> CreateRecipeWithIngredientsByIds(Guid userId, AddRecipeWithIngredientsDTO recipeDTO, CancellationToken ct)
        {
            // get only those present in the database
            var presentIngredientIds = _ingredients
                .Where(x => recipeDTO.Ingredients
                    .Select(x => x.IngredientId)
                    .Contains(x.Id))
                .Select(x => x.Id);

            var recipeIngredients = recipeDTO.Ingredients
                .Where(x => presentIngredientIds.Contains(x.IngredientId))
                .Distinct();

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var recipe = new Recipe
                {
                    Name = recipeDTO.Name,
                    Description = recipeDTO.Description,
                    PostingUserId = userId
                };

                await _recipes.AddAsync(recipe, ct);
                await _dbContext.SaveChangesAsync(ct);

                var recipeIngs = recipeIngredients
                    .Select(ri => new RecipeIngredient
                        {
                            RecipeId = recipe.Id,
                            IngredientId = ri.IngredientId,
                            Quantity = ri.Quantity,
                            UnitId = ri.UnitId
                        })
                    .ToList();

                await _recipeIngredients.AddRangeAsync(recipeIngs, ct);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return recipe.Id;
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(CreateRecipeWithIngredientsByIds)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetAllFullRecipes(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct)
        {
            IQueryable<Recipe> recipes = _recipes;

            // query
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                recipes = _recipes.Where(recipe => recipe.Name.ToUpper().Contains(query));
            }

            // order
            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            // count
            var totalCount = await recipes.CountAsync(ct);

            // project
            var recipeDTOs = await recipes
                .Skip((page - 1) * count)
                .Take(count)
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(ing => ing.Ingredient)
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(ing => ing.Unit)
                .Select(recipe => new
                {
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    Ingredients = recipe.Ingredients
                        .Select(ingredient => new GetRecipeIngredientDTO(
                        ingredient.Ingredient.Id,
                        ingredient.Ingredient.Name,
                        ingredient.Ingredient.Description ?? "",
                        ingredient.Quantity,
                        ingredient.UnitId,
                        ingredient.Unit.Name)).ToArray(),
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
                        userInfoTasks[recipe.PostingUserId].Result.ImageUrl)))
                .ToList();


            return new PaginatedResult<IEnumerable<GetFullRecipeDTO>> 
            { 
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetAllRecipes(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct)
        {
            IQueryable<Recipe> recipes = _recipes;

            // query
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                recipes = _recipes.Where(recipe => recipe.Name.ToUpper().Contains(query));
            }

            // order
            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            // count
            int totalCount = await recipes.CountAsync(ct);

            // project
            var recipeDTOs = await recipes
                .Skip((page - 1) * count)
                .Take(count)
                .Select(recipe => new
                {
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
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
                .Select(recipe => new GetRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        userInfoTasks[recipe.PostingUserId].Result.Username,
                        userInfoTasks[recipe.PostingUserId].Result.ImageUrl)))
                .ToList();

            return new PaginatedResult<IEnumerable<GetRecipeDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task<GetFullRecipeDTO> GetFullRecipeById(Guid userId, Guid recipeId, CancellationToken ct)
        {
            var recipe = await _recipes
                .Include(r => r.Ingredients)
                    .ThenInclude(r => r.Ingredient)
                .Include(r => r.Ingredients)
                    .ThenInclude(r => r.Unit)
                .FirstOrDefaultAsync(r => r.Id == recipeId, ct) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            var user = await _userInfoService.GetUserById(recipe.PostingUserId, userId);

            return new GetFullRecipeDTO(
                recipe.Id,
                recipe.Name,
                recipe.Description,
                recipe.Ingredients
                    .Select(i => new GetRecipeIngredientDTO(
                        i.Ingredient.Id,
                        i.Ingredient.Name,
                        i.Ingredient.Description ?? "",
                        i.Quantity,
                        i.UnitId,
                        i.Unit.Name))
                    .ToArray(),
                new CommonUserDataDTO(
                    recipe.PostingUserId,
                    user.Username,
                    user.ImageUrl));
        }

        //public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct)
        //{
        //    IQueryable<Recipe> recipes = _recipes;

        //    // query
        //    recipes = recipes.Where(recipe => recipeIds.Contains(recipe.Id));

        //    // order
        //    if (_recipeProps.Contains(sortBy))
        //    {
        //        recipes = recipes.OrderBy(sortBy, orderByAsc);
        //    }
        //    else
        //    {
        //        recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
        //    }

        //    // count
        //    int totalCount = await recipes.CountAsync(ct);

        //    // project
        //    var recipeDTOs = await recipes
        //        .Include(recipe => recipe.Ingredients)
        //            .ThenInclude(recipe => recipe.Ingredient)
        //        .Include(recipe => recipe.Ingredients)
        //            .ThenInclude(recipe => recipe.Unit)
        //        .Select(recipe => new
        //        {
        //            recipe.Id,
        //            recipe.Name,
        //            recipe.Description,
        //            Ingredients = recipe.Ingredients
        //                .Select(ingredient => new GetRecipeIngredientDTO(
        //                ingredient.Ingredient.Id,
        //                ingredient.Ingredient.Name,
        //                ingredient.Ingredient.Description ?? "",
        //                ingredient.Quantity,
        //                ingredient.UnitId,
        //                ingredient.Unit.Name)),
        //            recipe.PostingUserId
        //        })
        //        .ToListAsync(ct);


        //    var userIds = recipeDTOs.Select(r => r.PostingUserId).Distinct();

        //    // fetch all user info in parallel (or use a batch API if available)
        //    var userInfoTasks = userIds
        //        .ToDictionary(
        //            id => id,
        //            id => _userInfoService.GetUserById(id)
        //        );

        //    await Task.WhenAll(userInfoTasks.Values);

        //    var data = recipeDTOs
        //        .Select(recipe => new GetFullRecipeDTO(
        //            recipe.Id,
        //            recipe.Name,
        //            recipe.Description,
        //            recipe.Ingredients,
        //            new CommonUserDataDTO(
        //                recipe.PostingUserId,
        //                userInfoTasks[recipe.PostingUserId].Result.Username,
        //                userInfoTasks[recipe.PostingUserId].Result.ImageUrl)))
        //        .ToList();

        //    return new PaginatedResult<IEnumerable<GetFullRecipeDTO>>
        //    {
        //        Data = data,
        //        TotalElements = totalCount,
        //        TotalPages = (int)Math.Ceiling((double)totalCount / count),
        //        Page = page,
        //        PageSize = count
        //    };
        //}

        //public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIngredientIds(IEnumerable<Guid> ingredientIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct)
        //{
        //    IQueryable<Recipe> recipes = _recipes;

        //    // order
        //    if (_recipeProps.Contains(sortBy))
        //    {
        //        recipes = recipes.OrderBy(sortBy, orderByAsc);
        //    }
        //    else
        //    {
        //        recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
        //    }

        //    // count
        //    int totalCount = await recipes.CountAsync(ct);

        //    // project
        //    var recipeDTOs = await recipes
        //        .Include(recipe => recipe.Ingredients.Where(ingredient => ingredientIds.Contains(ingredient.IngredientId)))
        //            .ThenInclude(recipe => recipe.Ingredient)
        //        .Include(recipe => recipe.Ingredients.Where(ingredient => ingredientIds.Contains(ingredient.IngredientId)))
        //            .ThenInclude(recipe => recipe.Unit)
        //        .Skip(page * count)
        //        .Take(count)
        //        .Select(recipe => new
        //        {
        //            recipe.Id,
        //            recipe.Name,
        //            Description = recipe.Description ?? "",
        //            Ingredients = recipe.Ingredients
        //                .Select(recipeIngredient => new GetRecipeIngredientDTO(
        //                    recipeIngredient.IngredientId,
        //                    recipeIngredient.Ingredient.Name,
        //                    recipeIngredient.Ingredient.Description ?? "",
        //                    recipeIngredient.Quantity,
        //                    recipeIngredient.UnitId,
        //                    recipeIngredient.Unit.Name)),
        //            recipe.PostingUserId
        //        })
        //        .ToListAsync(ct);

        //    var userIds = recipeDTOs.Select(r => r.PostingUserId).Distinct();

        //    // fetch all user info in parallel (or use a batch API if available)
        //    var userInfoTasks = userIds
        //        .ToDictionary(
        //            id => id,
        //            id => _userInfoService.GetUserById(id)
        //        );

        //    await Task.WhenAll(userInfoTasks.Values);

        //    var data = recipeDTOs
        //        .Select(recipe => new GetFullRecipeDTO(
        //            recipe.Id,
        //            recipe.Name,
        //            recipe.Description,
        //            recipe.Ingredients,
        //            new CommonUserDataDTO(
        //                recipe.PostingUserId,
        //                userInfoTasks[recipe.PostingUserId].Result.Username,
        //                userInfoTasks[recipe.PostingUserId].Result.ImageUrl)))
        //        .ToList();

        //    return new PaginatedResult<IEnumerable<GetFullRecipeDTO>>
        //    {
        //        Data = data,
        //        TotalElements = totalCount,
        //        TotalPages = (int)Math.Ceiling((double)totalCount / count),
        //        Page = page,
        //        PageSize = count
        //    };
        //}

        public async Task<GetRecipeDTO> GetRecipeById(Guid userId, Guid recipeId, CancellationToken ct)
        {
            var recipe = await _recipes
                .FirstOrDefaultAsync(r => r.Id == recipeId, ct) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            var user = await _userInfoService.GetUserById(recipe.PostingUserId, userId);

            return new GetRecipeDTO(
                recipeId, 
                recipe.Name, 
                recipe.Description, 
                new CommonUserDataDTO(
                    recipe.PostingUserId,
                    user.Username,
                    user.ImageUrl));
        }

        //public async Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct)
        //{
        //    // query
        //    var recipes = _recipes.Where(x => recipeIds.Contains(x.Id));

        //    // order
        //    if (_recipeProps.Contains(sortBy))
        //    {
        //        recipes = recipes.OrderBy(sortBy, orderByAsc);
        //    }
        //    else
        //    {
        //        // by name by default
        //        recipes = orderByAsc ? recipes.OrderBy(x => x.Name) : recipes.OrderByDescending(x => x.Name);
        //    }

        //    // count
        //    int totalCount = await recipes.CountAsync(ct);

        //    // project
        //    var recipeDTOs = await recipes
        //        .Skip(page * count)
        //        .Take(count)
        //        .Select(recipe =>
        //            new
        //            {
        //                recipe.Id,
        //                recipe.Name,
        //                recipe.Description,
        //                recipe.PostingUserId,
        //            })
        //        .ToListAsync(ct);

        //    var userIds = recipeDTOs.Select(r => r.PostingUserId).Distinct();

        //    // fetch all user info in parallel (or use a batch API if available)
        //    var userInfoTasks = userIds
        //        .ToDictionary(
        //            id => id,
        //            id => _userInfoService.GetUserById(id)
        //        );

        //    await Task.WhenAll(userInfoTasks.Values);

        //    var data = recipeDTOs
        //        .Select(recipe => new GetRecipeDTO(
        //            recipe.Id,
        //            recipe.Name,
        //            recipe.Description,
        //            new CommonUserDataDTO(
        //                recipe.PostingUserId,
        //                userInfoTasks[recipe.PostingUserId].Result.Username,
        //                userInfoTasks[recipe.PostingUserId].Result.ImageUrl)))
        //        .ToList();


        //    return new PaginatedResult<IEnumerable<GetRecipeDTO>>
        //    {
        //        Data = data,
        //        TotalElements = totalCount,
        //        TotalPages = (int)Math.Ceiling((double)totalCount / count),
        //        Page = page,
        //        PageSize = count
        //    };
        //}

        // there may be added quantity and unit
        public async Task<GetRecipeWithIngredientsAndCategoriesDTO> GetRecipeWithIngredientsAndCategories(Guid userId, Guid recipeId, CancellationToken ct)
        {
            var recipe = await _recipes
                .Include(r => r.Ingredients)
                    .ThenInclude(i => i.Ingredient)
                        .ThenInclude(i => i.Connections)
                            .ThenInclude(c => c.IngredientCategory)
                .FirstOrDefaultAsync(r => r.Id == recipeId, ct) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            var user = await _userInfoService.GetUserById(recipe.PostingUserId, userId);

            return new GetRecipeWithIngredientsAndCategoriesDTO(
                recipe.Id,
                recipe.Name,
                recipe.Description,
                recipe.Ingredients
                    .Select(ingredient => new GetFullIngredientDataDTO(
                        ingredient.Ingredient.Id,
                        ingredient.Ingredient.Name,
                        ingredient.Ingredient.Description ?? "",
                        ingredient.Ingredient.Connections
                        .Select(category => new GetIngredientCategoryDTO(
                            category.IngredientCategory.Id,
                            category.IngredientCategory.Name,
                            category.IngredientCategory.Description))
                        .ToArray()
                        ))
                    .ToArray(),
                new CommonUserDataDTO(
                    recipe.PostingUserId,
                    user.Username,
                    user.ImageUrl));
        }

        public async Task AddIngredientToRecipeById(Guid userId, Guid recipeId, AddIngredientToRecipeDTO addIngredientDTO, CancellationToken ct)
        {
            if (!_recipes.Any(r => r.Id == recipeId && r.PostingUserId == userId))
            {
                throw new RecipeNotFoundException($"Recipe with id {recipeId} not found for user with id {userId}.");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                await _recipeIngredients.AddAsync(new RecipeIngredient()
                {
                    RecipeId = recipeId,
                    IngredientId = addIngredientDTO.IngredientId,
                    Quantity = addIngredientDTO.Quantity,
                    UnitId = addIngredientDTO.UnitId,
                }, ct);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddIngredientToRecipeById)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<IEnumerable<Guid>> AddIngredientsToRecipeById(Guid userId, Guid recipeId, AddIngredientRangeToRecipeDTO addIngredientsDTO, CancellationToken ct)
        {
            if (!_recipes.Any(r => r.Id == recipeId && r.PostingUserId == userId))
            {
                throw new RecipeNotFoundException($"Recipe with id {recipeId} not found for user with id {userId}.");
            }

            // only the ingredients that exist, take recipes of the ingredient that are not this recipe so the ingredients are ok
            var existingIngredients = addIngredientsDTO.Ingredients.Where(x => _ingredients.Any(y => y.Id == x.IngredientId));

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var recipeIngredients = existingIngredients.Select(ri => new RecipeIngredient()
                {
                    RecipeId = recipeId,
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    UnitId = ri.UnitId
                }).ToList();

                await _recipeIngredients.AddRangeAsync(recipeIngredients, ct);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return existingIngredients.Select(x => x.IngredientId);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddIngredientsToRecipeById)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        //public async Task<PaginatedResult<IEnumerable<GetRecipeWithIngredientIdsDTO>>> GetRecipesWithIngredientIdsByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct)
        //{
        //    // query
        //    var recipes = _recipes.Where(r => recipeIds.Contains(r.Id));

        //    // order
        //    if (_recipeProps.Contains(sortBy))
        //    {
        //        recipes = recipes.OrderBy(sortBy, orderByAsc);
        //    }
        //    else
        //    {
        //        // by name by default
        //        recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
        //    }

        //    // count
        //    int totalCount = await recipes.CountAsync(ct);

        //    // project
        //    var recipeDTOs = await recipes
        //        .Include(x => x.Ingredients)
        //        .Skip(page * count)
        //        .Take(count)
        //        .Select(recipe => new {
        //            recipe.Id,
        //            recipe.Name,
        //            Description = recipe.Description ?? "",
        //            recipe.PostingUserId,
        //            IngredientIds = recipe.Ingredients.Select(r => r.IngredientId)})
        //        .ToListAsync(ct);

        //    var userIds = recipeDTOs.Select(r => r.PostingUserId).Distinct();

        //    // fetch all user info in parallel (or use a batch API if available)
        //    var userInfoTasks = userIds
        //        .ToDictionary(
        //            id => id,
        //            id => _userInfoService.GetUserById(id)
        //        );

        //    await Task.WhenAll(userInfoTasks.Values);

        //    var data = recipeDTOs
        //        .Select(recipe => new GetRecipeWithIngredientIdsDTO(
        //            recipe.Id,
        //            recipe.Name,
        //            recipe.Description,
        //            new CommonUserDataDTO(
        //                recipe.PostingUserId,
        //                userInfoTasks[recipe.PostingUserId].Result.Username,
        //                userInfoTasks[recipe.PostingUserId].Result.ImageUrl),
        //            recipe.IngredientIds))
        //        .ToList();

        //    return new PaginatedResult<IEnumerable<GetRecipeWithIngredientIdsDTO>>
        //    {
        //        Data = data,
        //        TotalElements = totalCount,
        //        TotalPages = (int)Math.Ceiling((double)totalCount / count),
        //        Page = page,
        //        PageSize = count
        //    };
        //}

        public async Task RemoveIngredientFromRecipe(Guid userId, Guid recipeId, Guid ingredientId, CancellationToken ct)
        {
            var recipeIngredient = await _recipeIngredients
                .Include(x => x.Recipe)
                .Where(x => x.Recipe.PostingUserId == userId)
                .Where(x => x.RecipeId == recipeId)
                .Where(x => x.IngredientId == ingredientId)
                .FirstOrDefaultAsync(ct) ?? throw new ElementNotFoundException($"Ingredient with id {ingredientId} is not connected to recipe with id {recipeId}.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                _recipeIngredients.Remove(recipeIngredient);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveIngredientFromRecipe)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task RemoveIngredientsFromRecipe(Guid userId, Guid recipeId, IEnumerable<Guid> ingredientIds, CancellationToken ct)
        {
            var recipeIngredients = _recipeIngredients
                .Where(x => x.RecipeId == recipeId)
                .Include(x => x.Recipe)
                .Where(x => x.Recipe.PostingUserId == userId)
                .Where(x => ingredientIds.Contains(x.IngredientId));

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                _recipeIngredients.RemoveRange(recipeIngredients);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveIngredientsFromRecipe)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task RemoveRecipeById(Guid userId, Guid recipeId, CancellationToken ct)
        {
            var recipe = await _recipes
                .Where(x => x.PostingUserId == userId)
                .Where(x => x.Id == recipeId)
                .Include(x => x.Ingredients)
                .FirstOrDefaultAsync(ct) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            var recipeIngredients = _recipeIngredients.Where(x => x.RecipeId == recipeId);

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                _recipes.Remove(recipe);
                if (recipeIngredients.Any())
                {
                    _recipeIngredients.RemoveRange(recipeIngredients);
                }

                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveRecipeById)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task UpdateRecipeNameById(Guid userId, Guid recipeId, UpdateRecipeDTO recipeDTO, CancellationToken ct)
        {
            var recipe = await _recipes
                .FirstOrDefaultAsync(x => x.Id == recipeId && x.PostingUserId == userId, ct) 
                ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found for user with id {userId}.");

            // add validation in different layer

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                recipe.Name = recipeDTO.Name;
                recipe.Description = recipeDTO.Description;

                _recipes.Update(recipe);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(UpdateRecipeNameById)}. Rollback.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}
