using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Exceptions;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeDbContext _dbContext;
        private readonly DbSet<Recipe> _recipes;
        private readonly DbSet<RecipeIngredient> _recipeIngredients;
        private readonly DbSet<Ingredient> _ingredients;

        private readonly HashSet<string> _recipeProps;

        public RecipeService(RecipeDbContext dbContext)
        {
            _dbContext = dbContext;
            _recipeProps = typeof(Recipe)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet();

            _recipes = dbContext.Set<Recipe>();
            _ingredients = dbContext.Set<Ingredient>();
            _recipeIngredients = dbContext.Set<RecipeIngredient>();
            
        }

        public GetFullRecipeDTO[] GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy)
        {
            IOrderedQueryable<Recipe> res;

            if (_recipeProps.Contains(sortBy))
            {
                res = orderByAsc ? _recipes.OrderBy(x => sortBy) : _recipes.OrderByDescending(x => sortBy);
            }
            else
            {
                // by name by default
                res = orderByAsc ? _recipes.OrderBy(x => x.Name) : _recipes.OrderByDescending(x => x.Name);
            }

            var result = res
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

        public GetRecipeDTO[] GetAllRecipes(int count, int page, bool orderByAsc, string sortBy)
        {
            IOrderedQueryable<Recipe> res;

            if (_recipeProps.Contains(sortBy))
            {
                res = orderByAsc ? _recipes.OrderBy(x => sortBy) : _recipes.OrderByDescending(x => sortBy);
            }
            else
            {
                // by name by default
                res = orderByAsc ? _recipes.OrderBy(x => x.Name) : _recipes.OrderByDescending(x => x.Name);
            }

            var result = res
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

        public GetFullRecipeDTO[] GetFullRecipesByIds(Guid[] recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            var result = _recipes
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

        public GetFullRecipeDTO[] GetFullRecipesByIngredientIds(Guid[] ingredientIds, int count, int page, bool orderByAsc, string sortBy)
        {
            IOrderedQueryable<Recipe> res;

            if (_recipeProps.Contains(sortBy))
            {
                res = orderByAsc ? _recipes.OrderBy(x => sortBy) : _recipes.OrderByDescending(x => sortBy);
            }
            else
            {
                // by name by default
                res = orderByAsc ? _recipes.OrderBy(x => x.Name) : _recipes.OrderByDescending(x => x.Name);
            }

            return res
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

        public GetRecipeDTO[] GetRecipesByIds(Guid[] recipeIds, int count, int page, bool orderByAsc, string sortBy)
        {
            var res = _recipes.Where(x => recipeIds.Contains(x.Id));

            if (_recipeProps.Contains(sortBy))
            {
                res = orderByAsc ? res.OrderBy(x => sortBy) : res.OrderByDescending(x => sortBy);
            }
            else
            {
                // by name by default
                res = orderByAsc ? res.OrderBy(x => x.Name) : res.OrderByDescending(x => x.Name);
            }

            return res
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetRecipeDTO(recipe.Id, recipe.Name, recipe.Description))
                .ToArray();
        }

        public GetRecipeWithIngredientIdsDTO[] GetRecipesWithIngredientIdsByIds(Guid[] recipeIds, bool orderByAsc, string sortBy)
        {
            throw new NotImplementedException();
        }

        public GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId)
        {
            throw new NotImplementedException();
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
    }
}
