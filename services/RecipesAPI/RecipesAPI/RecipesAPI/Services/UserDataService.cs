using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Entities.UserData;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Extensions;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
using RecipesAPI.Model.UserData.Cookbook.Update;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Services
{
    public class UserDataService : IUserDataService
    {
        ILogger<UserDataService> _logger;

        private readonly RecipeDbContext _dbContext;
        private readonly DbSet<UserCookbookRecipe> _cookbookRecipes;

        private readonly HashSet<string> _recipeProps;

        public UserDataService(ILogger<UserDataService> logger, RecipeDbContext dbContext)
        {
            _logger = logger;

            _dbContext = dbContext;
            _cookbookRecipes = _dbContext.Set<UserCookbookRecipe>();

            _recipeProps = typeof(Recipe)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

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

        public IEnumerable<GetFullRecipeForCookbookDTO> GetFullUserCookbook(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, bool showOnlyFavorites)
        {
            var recipes = showOnlyFavorites ? _cookbookRecipes.Where(cr => cr.IsFavorite) : _cookbookRecipes;


            // search query
            recipes = string.IsNullOrEmpty(query) ? recipes : recipes
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

            var recipesFromCookbook = recipes
                .Where(cr => cr.UserId == userId)
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
                        cr.User.LastName)
                    )).ToArray();

            return recipesFromCookbook;
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
    }
}
