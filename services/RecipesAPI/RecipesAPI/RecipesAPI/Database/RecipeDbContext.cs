using Microsoft.EntityFrameworkCore;
using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Entities.UserData;

namespace RecipesAPI.Database
{
    public class RecipeDbContext : DbContext
    {
        public RecipeDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    
        // recipes & ingredients
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientCategoryConnection> IngredientCategoryConnections { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }

        // user data
        public DbSet<UserCookbookRecipes> UserCookbooks { get; set; }
        public DbSet<UserDietaryRestriction> UserDietaryRestrictions { get; set; }
        public DbSet<UserFridgeIngredients> UserFridges { get; set; }
    }
}
