using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Entities.UserData;

namespace RecipesAPI.Database
{
    public class RecipeDatabaseSeeder
    {
        private readonly ILogger<RecipeDatabaseSeeder> _logger;
        private readonly RecipeDbContext _dbContext;

        public RecipeDatabaseSeeder(ILogger<RecipeDatabaseSeeder> logger, RecipeDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Seed()
        {
            if (!await _dbContext.Database.CanConnectAsync())
            {
                _logger.LogError("Seeder cannot connect to the db.");
                return;
            }

            if (_dbContext.Set<Ingredient>().Any())
            {
                _logger.LogError("Ingredients already seeded.");
                return;
            }

            var ingredients = new List<Ingredient>(100);
            var categories = new List<IngredientCategory>(100);
            var connections = new List<IngredientCategoryConnection>(100);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var basepath = Path.Combine(Environment.CurrentDirectory, "Database");

            var testUser = new User()
            {
                FistName = "Testing",
                LastName = "Stilltesting",
                EmailAddress = "just@test.com",
                PasswordHash = "nohash:3"
            };

            await _dbContext.Set<User>().AddAsync(testUser);

            try
            {
                // read ingredients
                using (StreamReader sr = new StreamReader(Path.Combine(basepath, "seeding_data_ingredients.csv")))
                {
                    bool wasStartRead = false;
                    while (!sr.EndOfStream)
                    {
                        if (!wasStartRead)
                        {
                            if (sr.ReadLine() == "name;description")
                            {
                                wasStartRead = true;
                            }
                            continue;
                        }
                        var line = sr.ReadLine();
                        var split = line!.Split(';');
                        if (split.Length == 2)
                        {
                            var ingredient = new Ingredient()
                            {
                                Name = split[0],
                                Description = split[1]
                            };
                            ingredients.Add(ingredient);
                        }
                    }
                }

                await _dbContext.Set<Ingredient>().AddRangeAsync(ingredients);

                // read categories
                using (StreamReader sr = new StreamReader(Path.Combine(basepath, "seeding_data_categories.csv")))
                {
                    bool wasStartRead = false;
                    while (!sr.EndOfStream)
                    {
                        if (!wasStartRead)
                        {
                            if (sr.ReadLine() == "name;description")
                            {
                                wasStartRead = true;
                            }
                            continue;
                        }
                        var line = sr.ReadLine();
                        var split = line!.Split(';');
                        if (split.Length == 2)
                        {
                            var category = new IngredientCategory()
                            {
                                Name = split[0],
                                Description = split[1]
                            };
                            categories.Add(category);
                        }
                    }
                }

                await _dbContext.Set<IngredientCategory>().AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();

                // read ingredient_index -> category_index and create connections
                using (StreamReader sr = new StreamReader(Path.Combine(basepath, "seeding_data_connection_indexes.csv")))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var split = line!.Split(';');
                        if (split.Length == 2)
                        {
                            var connection = new IngredientCategoryConnection()
                            {
                                IngredientId = ingredients[int.Parse(split[0])].Id,
                                CategoryId = categories[int.Parse(split[1])].Id
                            };
                            connections.Add(connection);
                        }
                    }
                }

                await _dbContext.Set<IngredientCategoryConnection>().AddRangeAsync(connections);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
                await transaction.RollbackAsync();
            }
        }
    }
}
