using Microsoft.EntityFrameworkCore;
using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Entities.Reviews;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // recipes
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.PostingUser)
                .WithMany(r => r.UserRecipes)
                .HasForeignKey(r => r.PostingUserId);

            // ingredient category connections
            modelBuilder.Entity<IngredientCategoryConnection>()
                .HasKey(ic => new { ic.CategoryId, ic.IngredientId });

            modelBuilder.Entity<IngredientCategoryConnection>()
                .HasOne(ic => ic.IngredientCategory)
                .WithMany(ic => ic.Connections) 
                .HasForeignKey(ic => ic.CategoryId);

            modelBuilder.Entity<IngredientCategoryConnection>()
                .HasOne(ic => ic.Ingredient)
                .WithMany(ic => ic.Connections) 
                .HasForeignKey(ic => ic.IngredientId);

            // recipe ingredients
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(ri => ri.Ingredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(ri => ri.Recipes)
                .HasForeignKey(ri => ri.IngredientId);

            // user cookbooks
            modelBuilder.Entity<UserCookbookRecipe>()
                .HasKey(ucr => new { ucr.UserId, ucr.RecipeId });

            modelBuilder.Entity<UserCookbookRecipe>()
                .HasOne(ucr => ucr.Recipe)
                .WithMany(ucr => ucr.UserCookbooks)
                .HasForeignKey(ucr => ucr.RecipeId);

            modelBuilder.Entity<UserCookbookRecipe>()
                .HasOne(ucr => ucr.User)
                .WithMany(ucr => ucr.CookbookRecipes)
                .HasForeignKey(ucr => ucr.UserId);

            // user dietary restrictions
            modelBuilder.Entity<UserDietaryRestriction>()
                .HasKey(udr => new { udr.UserId, udr.IngredientCategoryId });

            modelBuilder.Entity<UserDietaryRestriction>()
                .HasOne(udr => udr.IngredientCategory)
                .WithMany(udr => udr.UserDietaryRestrictions)
                .HasForeignKey(udr => udr.IngredientCategoryId);

            modelBuilder.Entity<UserDietaryRestriction>()
                .HasOne(udr => udr.User)
                .WithMany(udr => udr.DietaryRestrictions)
                .HasForeignKey(udr => udr.UserId);

            // user fridge ingredients
            modelBuilder.Entity<UserFridgeIngredient>()
                .HasKey(ufi => new { ufi.UserId, ufi.IngredientId });

            modelBuilder.Entity<UserFridgeIngredient>()
                .HasOne(ufi => ufi.Ingredient)
                .WithMany(ufi => ufi.UserFridges)
                .HasForeignKey(ufi => ufi.IngredientId);

            modelBuilder.Entity<UserFridgeIngredient>()
                .HasOne(ufi => ufi.User)
                .WithMany(ufi => ufi.FridgeIngredients)
                .HasForeignKey(ufi => ufi.UserId);

            // reviews
            modelBuilder.Entity<Review>()
                .HasKey(rev => new { rev.UserId, rev.RecipeId });

            modelBuilder.Entity<Review>()
                .HasOne(rev => rev.Recipe)
                .WithMany(rev => rev.Reviews)
                .HasForeignKey(rev => rev.RecipeId);

            // reviewPhotos
            modelBuilder.Entity<ReviewPhoto>()
                .HasKey(revp => new { revp.ReviewId, revp.PhotoUrlId });

        }

        // recipes & ingredients
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientCategoryConnection> IngredientCategoryConnections { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }

        // user data
        public DbSet<User> Users { get; set; }
        public DbSet<UserCookbookRecipe> UserCookbooks { get; set; }
        public DbSet<UserDietaryRestriction> UserDietaryRestrictions { get; set; }
        public DbSet<UserFridgeIngredient> UserFridges { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        // reviews
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewPhoto> ReviewPhotos { get; set; }
        public DbSet<PhotoUrl> PhotoUrls { get; set; }
    }
}
