using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Exceptions.Duplicates;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Services.Interfaces;
using RecipesAPI.Extensions;

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
                .ToHashSet();

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
                    Description = recipeDTO.Description
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

        public async Task<Guid> CreateRecipeWithIngredientsByIds(Guid userId, AddRecipeWithIngredientIdsDTO recipeDTO)
        {
            // this may be unused as recipes can be done differently and posted by a different user
            if (_recipes.Any(recipe => recipe.Name.ToLower() == recipeDTO.Name.ToLower()))
            {
                throw new DuplicateRecipeException($"Recipe with name {recipeDTO.Name} already exists.");
            }

            var distinctIngredientIds = recipeDTO.IngredientIds.Distinct();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            // get only those present in the database
            var presentIngredientIds = _ingredients.Where(x => distinctIngredientIds.Contains(x.Id)).Select(x => x.Id);
            try
            {
                var recipe = new Recipe
                {
                    Name = recipeDTO.Name,
                    Description = recipeDTO.Description,
                };

                await _recipes.AddAsync(recipe);
                await _dbContext.SaveChangesAsync();

                foreach (var ingredientId in presentIngredientIds)
                {
                    await _recipeIngredients.AddAsync(new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = ingredientId,
                    });
                }

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

        public IEnumerable<GetFullRecipeDTO> GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy, string query)
        {
            // search query
            var recipes = string.IsNullOrEmpty(query)
                ? _recipes
                : _recipes.Where(recipe => recipe.Name.Contains(query));


            if (_recipeProps.Contains(sortBy))
            {
                var prop = typeof(Recipe).GetProperty(sortBy);
                recipes = orderByAsc ? recipes.OrderBy(r => prop) : recipes.OrderByDescending(r => prop);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            var result = recipes
                .Skip(page * count)
                .Take(count)
                .Include(recipe => recipe.Ingredients)
                .Select(recipe => new GetFullRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    recipe.Ingredients
                        .Join(_ingredients, x => x.IngredientId, y => y.Id, (x, y) => y)
                        .Select(ingredient => new GetIngredientDTO(
                        ingredient.Id,
                        ingredient.Name,
                        ingredient.Description)).ToArray())).ToArray();

            return result;
        }

        public IEnumerable<GetRecipeDTO> GetAllRecipes(int count, int page, bool orderByAsc, string sortBy, string query)
        {
            // search query
            var recipes = string.IsNullOrEmpty(query)
                ? _recipes
                : _recipes.Where(recipe => recipe.Name.Contains(query));

            // sorting
            if (_recipeProps.Contains(sortBy))
            {
                var prop = typeof(Recipe).GetProperty(sortBy);
                recipes = recipes.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            // pagination
            var result = recipes
                .Skip(page * count)
                .Take(count)
                .Include(recipe => recipe.Ingredients)
                .Select(recipe => new GetRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description))
                .ToArray();

            return result;
        }

        public GetFullRecipeDTO GetFullRecipeById(Guid recipeId)
        {
            var recipe = _recipes
                .Include(r => r.Ingredients)
                .ThenInclude(r => r.Ingredient)
                .FirstOrDefault(r => r.Id == recipeId) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            return new GetFullRecipeDTO(
                recipe.Id,
                recipe.Name,
                recipe.Description,
                recipe.Ingredients
                    .Join(_ingredients,
                        ri => ri.IngredientId,
                        i => i.Id,
                        (ri, i) => i)
                    .Select(i => new GetIngredientDTO(
                        i.Id,
                        i.Name,
                        i.Description))
                    .ToArray());
        }

        public IEnumerable<GetFullRecipeDTO> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            IQueryable<Recipe> recipes = _recipes;

            if (_recipeProps.Contains(sortBy))
            {
                var prop = typeof(Recipe).GetProperty(sortBy);
                recipes = orderByAsc ? recipes.OrderBy(r => prop) : recipes.OrderByDescending(r => prop);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            var result = recipes
                .Where(recipe => recipeIds.Contains(recipe.Id))
                .OrderBy(recipe => recipe.Name)
                .Include(recipe => recipe.Ingredients)
                .ThenInclude(recipe => recipe.Ingredient)
                .Select(recipe => new GetFullRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    recipe.Ingredients
                        .Select(ingredient => new GetIngredientDTO(
                        ingredient.Ingredient.Id,
                        ingredient.Ingredient.Name,
                        ingredient.Ingredient.Description))
                        .ToArray()))
                .ToArray();

            return result;
        }

        public IEnumerable<GetFullRecipeDTO> GetFullRecipesByIngredientIds(IEnumerable<Guid> ingredientIds, int count, int page, bool orderByAsc, string sortBy)
        {
            IQueryable<Recipe> recipes = _recipes;

            if (_recipeProps.Contains(sortBy))
            {
                var prop = typeof(Recipe).GetProperty(sortBy);
                recipes = orderByAsc ? recipes.OrderBy(r => prop) : recipes.OrderByDescending(r => prop);
            }
            else
            {
                recipes = orderByAsc ? recipes.OrderBy(r => r.Name) : recipes.OrderByDescending(r => r.Name);
            }

            return recipes
                .Include(recipe => recipe.Ingredients.Where(ingredient => ingredientIds.Contains(ingredient.IngredientId)))
                .ThenInclude(recipe => recipe.Ingredient)
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetFullRecipeDTO(
                    recipe.Id,
                    recipe.Name,
                    recipe.Description,
                    recipe.Ingredients
                        .Select(recipeIngredient => new GetIngredientDTO(
                            recipeIngredient.IngredientId,
                            recipeIngredient.Ingredient.Name,
                            recipeIngredient.Ingredient.Description))
                        .ToArray()))
                .ToArray(); 
        }

        public GetRecipeDTO GetRecipeById(Guid recipeId)
        {
            var recipe = _recipes
                .Include(r => r.Ingredients)
                .ThenInclude(r => r.Ingredient)
                .FirstOrDefault(r => r.Id == recipeId) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            return new GetRecipeDTO(recipeId, recipe.Name, recipe.Description);
        }

        public IEnumerable<GetRecipeDTO> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            var result = _recipes.Where(x => recipeIds.Contains(x.Id));

            if (_recipeProps.Contains(sortBy))
            {
                var prop = typeof(Recipe).GetProperty(sortBy);
                result = orderByAsc ? result.OrderBy(r => prop) : result.OrderByDescending(r => prop);
            }
            else
            {
                // by name by default
                result = orderByAsc ? result.OrderBy(x => x.Name) : result.OrderByDescending(x => x.Name);
            }

            return result
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetRecipeDTO(recipe.Id, recipe.Name, recipe.Description))
                .ToArray();
        }

        public GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId)
        {
            var recipe = _recipes
                .Include(r => r.Ingredients)
                .FirstOrDefault(r => r.Id == recipeId) ?? throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");

            return new GetRecipeWithIngredientIdsDTO(recipe.Id, recipe.Name, recipe.Description, recipe.Ingredients.Select(x => x.IngredientId));
        }

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
                        ingredient.Ingredient.Description,
                        ingredient.Ingredient.Connections
                        .Select(category => new GetIngredientCategoryDTO(
                            category.IngredientCategory.Id,
                            category.IngredientCategory.Name,
                            category.IngredientCategory.Description))
                        .ToArray()
                        ))
                    .ToArray());
        }

        public async Task AddIngredientToRecipeById(Guid recipeId, AddIngredientToRecipeDTO ingredientDTO)
        {
            // these exceptions will be thrown by database as this is a primary key
            //if (!_recipes.Any(r => r.Id == recipeId))
            //{
            //    throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");
            //}

            //if (!_ingredients.Any(i => i.Id == ingredientDTO.IngredientId))
            //{
            //    throw new IngredientNotFoundException($"Recipe with id {recipeId} not found.");
            //}

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _recipeIngredients.AddAsync(new RecipeIngredient()
                {
                    RecipeId = recipeId,
                    IngredientId = ingredientDTO.IngredientId
                });
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddIngredientsToRecipeById(Guid recipeId, AddIngredientRangeToRecipeDTO ingredientDTO)
        {
            if (!_recipes.Any(r => r.Id == recipeId))
            {
                throw new RecipeNotFoundException($"Recipe with id {recipeId} not found.");
            }

            // only the ingredients that exist, take recipes of the ingredient that are not this recipe so the ingredients are ok
            var existingIngredientIds = _ingredients
                .Where(x => ingredientDTO.IngredientIds.Contains(x.Id))
                .Include(x => x.Recipes)
                .Where(x => !x.Recipes.Any(y => y.RecipeId == recipeId))
                .Select(x => x.Id)
                .ToArray();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var recipeIngredients = new List<RecipeIngredient>(existingIngredientIds.Length);

                foreach (var ingredientId in existingIngredientIds)
                {
                    var recipeIngredient = new RecipeIngredient()
                    {
                        IngredientId = ingredientId,
                        RecipeId = recipeId,
                    };
                    recipeIngredients.Add(recipeIngredient);
                }

                await _recipeIngredients.AddRangeAsync(recipeIngredients);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                _logger.LogError($"Issue with transaction {transaction.TransactionId}. Rollback.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public IEnumerable<GetRecipeWithIngredientIdsDTO> GetRecipesWithIngredientIdsByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            var result = _recipes.Where(r => recipeIds.Contains(r.Id));

            if (_recipeProps.Contains(sortBy))
            {
                var prop = typeof(Recipe).GetProperty(sortBy);
                result = orderByAsc ? result.OrderBy(r => prop) : result.OrderByDescending(r => prop);
            }
            else
            {
                // by name by default
                result = orderByAsc ? result.OrderBy(r => r.Name) : result.OrderByDescending(r => r.Name);
            }

            return result
                .Include(x => x.Ingredients)
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetRecipeWithIngredientIdsDTO(recipe.Id, recipe.Name, recipe.Description, recipe.Ingredients.Select(r => r.IngredientId)))
                .ToArray();
        }
    }
}
