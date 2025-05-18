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

        private readonly RecipeDbContext _dbContext;
        private readonly DbSet<Recipe> _recipes;
        private readonly DbSet<RecipeIngredient> _recipeIngredients;
        private readonly DbSet<Ingredient> _ingredients;

        private readonly HashSet<string> _recipeProps;

        public RecipeService(ILogger<RecipeService> logger, RecipeDbContext dbContext)
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
            
        }

        public async Task<Guid> CreateRecipe(Guid userId, AddRecipeDTO recipeDTO)
        {

            if (_recipes.Where(x => x.PostingUserId == userId).Any(recipe => recipe.Name.ToLower() == recipeDTO.Name.ToLower()))
            {
                throw new DuplicateRecipeException($"Recipe with name {recipeDTO.Name} already exists.");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var recipe = new Recipe
                {
                    Name = recipeDTO.Name,
                    Description = recipeDTO.Description,
                    PostingUserId = userId
                };

                await _recipes.AddAsync(recipe);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return recipe.Id;
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<Guid> CreateRecipeWithIngredientsByNames(Guid userId, AddRecipeWithIngredientNamesDTO recipeDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> CreateRecipeWithIngredientsByIds(Guid userId, AddRecipeWithIngredientsDTO recipeDTO)
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

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var recipe = new Recipe
                {
                    Name = recipeDTO.Name,
                    Description = recipeDTO.Description,
                    PostingUserId = userId
                };

                await _recipes.AddAsync(recipe);
                await _dbContext.SaveChangesAsync();

                foreach (var recipeIngredient in recipeIngredients)
                {
                    await _recipeIngredients.AddAsync(new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = recipeIngredient.IngredientId,
                        Quantity = recipeIngredient.Quantity,
                        UnitId = recipeIngredient.UnitId
                    });
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return recipe.Id;
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(CreateRecipeWithIngredientsByIds)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy, string query)
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
            var totalCount = await recipes.CountAsync();

            // project
            var data = await recipes
                .Skip(page * count)
                .Take(count)
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(ing => ing.Ingredient)
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(ing => ing.Unit)
                .Select(recipe => new GetFullRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    recipe.Ingredients
                        .Select(ingredient => new GetRecipeIngredientDTO(
                        ingredient.Ingredient.Id,
                        ingredient.Ingredient.Name,
                        ingredient.Ingredient.Description ?? "",
                        ingredient.Quantity,
                        ingredient.UnitId,
                        ingredient.Unit.Name))
                        .ToArray(),
                    new CommonUserDataDTO( // users stored in different database
                        recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User")))
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

        public async Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetAllRecipes(int count, int page, bool orderByAsc, string sortBy, string query)
        {
            IQueryable<Recipe> recipes = _recipes;

            // query
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToUpper();
                recipes = _recipes.Where(recipe => recipe.Name.ToUpper().Contains(query));
            }

            // sorting
            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            // count
            int totalCount = await recipes.CountAsync();

            // project
            var data = await recipes
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User")))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetRecipeDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public GetFullRecipeDTO GetFullRecipeById(Guid recipeId)
        {
            var recipe = _recipes
                .Include(r => r.Ingredients)
                    .ThenInclude(r => r.Ingredient)
                .Include(r => r.Ingredients)
                    .ThenInclude(r => r.Unit)
                .FirstOrDefault(r => r.Id == recipeId) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

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
                    "Temporary",
                    "Disabled",
                    "Posting User"));
        }

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            IQueryable<Recipe> recipes = _recipes;

            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            // query
            recipes = recipes.Where(recipe => recipeIds.Contains(recipe.Id));

            // count
            int totalCount = await recipes.CountAsync();

            // project
            var data = await recipes
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(recipe => recipe.Ingredient)
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(recipe => recipe.Unit)
                .Select(recipe => new GetFullRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    recipe.Ingredients
                        .Select(ingredient => new GetRecipeIngredientDTO(
                        ingredient.Ingredient.Id,
                        ingredient.Ingredient.Name,
                        ingredient.Ingredient.Description ?? "",
                        ingredient.Quantity,
                        ingredient.UnitId,
                        ingredient.Unit.Name)),
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User")))
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

        public async Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIngredientIds(IEnumerable<Guid> ingredientIds, int count, int page, bool orderByAsc, string sortBy)
        {
            IQueryable<Recipe> recipes = _recipes;

            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            // count
            int totalCount = await recipes.CountAsync();

            // project
            var data = await recipes
                .Include(recipe => recipe.Ingredients.Where(ingredient => ingredientIds.Contains(ingredient.IngredientId)))
                    .ThenInclude(recipe => recipe.Ingredient)
                .Include(recipe => recipe.Ingredients.Where(ingredient => ingredientIds.Contains(ingredient.IngredientId)))
                    .ThenInclude(recipe => recipe.Unit)
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetFullRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description ?? "",
                    recipe.Ingredients
                        .Select(recipeIngredient => new GetRecipeIngredientDTO(
                            recipeIngredient.IngredientId,
                            recipeIngredient.Ingredient.Name,
                            recipeIngredient.Ingredient.Description ?? "",
                            recipeIngredient.Quantity,
                            recipeIngredient.UnitId,
                            recipeIngredient.Unit.Name)),
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User")))
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

        public GetRecipeDTO GetRecipeById(Guid recipeId)
        {
            var recipe = _recipes
                .FirstOrDefault(r => r.Id == recipeId) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            return new GetRecipeDTO(
                recipeId, 
                recipe.Name, 
                recipe.Description, 
                new CommonUserDataDTO(
                    recipe.PostingUserId,
                    "Temporary",
                    "Disabled",
                    "Posting User"));
        }

        public async Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            // query
            var recipes = _recipes.Where(x => recipeIds.Contains(x.Id));

            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                // by name by default
                recipes = orderByAsc ? recipes.OrderBy(x => x.Name) : recipes.OrderByDescending(x => x.Name);
            }

            // count
            int totalCount = await recipes.CountAsync();

            // project
            var data = await recipes
                .Skip(page * count)
                .Take(count)
                .Select(recipe =>
                    new GetRecipeDTO(
                        recipe.Id,
                        recipe.Name,
                        recipe.Description,
                        new CommonUserDataDTO(
                            recipe.PostingUserId,
                            "Temporary",
                            "Disabled",
                            "Posting User")))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetRecipeDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId)
        {
            var recipe = _recipes
                .Include(r => r.Ingredients)
                .FirstOrDefault(r => r.Id == recipeId) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            return new GetRecipeWithIngredientIdsDTO(
                recipe.Id, 
                recipe.Name, 
                recipe.Description,
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User"),
                recipe.Ingredients.Select(x => x.IngredientId));
        }

        // there may be added quantity and unit
        public GetRecipeWithIngredientsAndCategoriesDTO GetRecipeWithIngredientsAndCategories(Guid recipeId)
        {
            var recipe = _recipes
                .Include(r => r.Ingredients)
                    .ThenInclude(i => i.Ingredient)
                        .ThenInclude(i => i.Connections)
                            .ThenInclude(c => c.IngredientCategory)
                .FirstOrDefault(r => r.Id == recipeId) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

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
                        "Temporary",
                        "Disabled",
                        "Posting User"));
        }

        public async Task AddIngredientToRecipeById(Guid recipeId, AddIngredientToRecipeDTO addIngredientDTO)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _recipeIngredients.AddAsync(new RecipeIngredient()
                {
                    RecipeId = recipeId,
                    IngredientId = addIngredientDTO.IngredientId,
                    Quantity = addIngredientDTO.Quantity,
                    UnitId = addIngredientDTO.UnitId,
                });
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddIngredientToRecipeById)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Guid>> AddIngredientsToRecipeById(Guid recipeId, AddIngredientRangeToRecipeDTO addIngredientsDTO)
        {
            if (!_recipes.Any(r => r.Id == recipeId))
            {
                throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");
            }

            // only the ingredients that exist, take recipes of the ingredient that are not this recipe so the ingredients are ok
            var existingIngredients = addIngredientsDTO.Ingredients.Where(x => _ingredients.Any(y => y.Id == x.IngredientId));
            //var existingIngredients = _ingredients
            //    .Where(x => addIngredientsDTO.Ingredients.Select(x => x.IngredientId).Contains(x.Id))
            //    .Include(x => x.Recipes)
            //    .Where(x => !x.Recipes.Any(y => y.RecipeId == recipeId));

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var recipeIngredients = new List<RecipeIngredient>(addIngredientsDTO.Ingredients.Count());

                foreach (var ingredient in existingIngredients)
                {
                    var recipeIngredient = new RecipeIngredient()
                    {
                        IngredientId = ingredient.IngredientId,
                        RecipeId = recipeId,
                        Quantity = ingredient.Quantity,
                        UnitId = ingredient.UnitId,
                    };
                    recipeIngredients.Add(recipeIngredient);
                }

                await _recipeIngredients.AddRangeAsync(recipeIngredients);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return existingIngredients.Select(x => x.IngredientId);
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(AddIngredientsToRecipeById)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginatedResult<IEnumerable<GetRecipeWithIngredientIdsDTO>>> GetRecipesWithIngredientIdsByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            // query
            var recipes = _recipes.Where(r => recipeIds.Contains(r.Id));

            if (_recipeProps.Contains(sortBy))
            {
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                // by name by default
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            // count
            int totalCount = await recipes.CountAsync();

            // project
            var data = await recipes
                .Include(x => x.Ingredients)
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetRecipeWithIngredientIdsDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description ?? "",
                    new CommonUserDataDTO(
                        recipe.PostingUserId,
                        "Temporary",
                        "Disabled",
                        "Posting User"),
                    recipe.Ingredients.Select(r => r.IngredientId)))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetRecipeWithIngredientIdsDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task RemoveIngredientFromRecipe(Guid recipeId, Guid ingredientId)
        {
            var recipeIngredient = _recipeIngredients
                .Where(x => x.RecipeId == recipeId)
                .Where(x => x.IngredientId == ingredientId)
                .FirstOrDefault() ?? throw new ElementNotFoundException($"Ingredient with id {ingredientId} is not connected to recipe with id {recipeId}.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _recipeIngredients.Remove(recipeIngredient);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveIngredientFromRecipe)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveIngredientsFromRecipe(Guid recipeId, IEnumerable<Guid> ingredientIds)
        {
            var recipeIngredients = _recipeIngredients
                .Where(x => x.RecipeId == recipeId)
                .Where(x => ingredientIds.Contains(x.IngredientId));

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _recipeIngredients.RemoveRange(recipeIngredients);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveIngredientsFromRecipe)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveRecipeById(Guid recipeId)
        {
            var recipe = _recipes
                .Where(x => x.Id == recipeId)
                .Include(x => x.Ingredients)
                .FirstOrDefault() ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            var recipeIngredients = _recipeIngredients.Where(x => x.RecipeId == recipeId);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _recipes.Remove(recipe);
                if (recipeIngredients.Any())
                {
                    _recipeIngredients.RemoveRange(recipeIngredients);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(RemoveRecipeById)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateRecipeNameById(Guid recipeId, UpdateRecipeDTO recipeDTO)
        {
            var recipe = _recipes
                .FirstOrDefault(x => x.Id == recipeId) 
                ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            // add validation in different layer

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                recipe.Name = recipeDTO.Name;
                recipe.Description = recipeDTO.Description;

                _recipes.Update(recipe);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId} at action {nameof(UpdateRecipeNameById)}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
