using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database;
using RecipesAPI.Entities.Ingredients;
using RecipesAPI.Exceptions.Duplicates;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Ingredients.Add;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Services.Interfaces;
using RecipesAPI.Extensions;
using RecipesAPI.Model.Common;
using RecipesAPI.Model.Units.Get;
using RecipesAPI.Model.Units.Request;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Services
{
    public class IngredientService : IIngredientService
    {
        private ILogger<IngredientService> _logger;

        private readonly RecipeDbContext _dbContext;
        private readonly DbSet<Ingredient> _ingredients;
        private readonly DbSet<IngredientCategory> _categories;
        private readonly DbSet<IngredientCategoryConnection> _connections;
        private readonly DbSet<Unit> _units;
        private readonly DbSet<IngredientUnitsRatio> _unitRatios;

        private readonly HashSet<string> _ingredientProps;
        private readonly HashSet<string> _unitProps;

        public IngredientService(ILogger<IngredientService> logger, RecipeDbContext dbContext)
        {
            _logger = logger;
            
            _dbContext = dbContext;
            _ingredients = dbContext.Set<Ingredient>();
            _categories = dbContext.Set<IngredientCategory>();
            _connections = dbContext.Set<IngredientCategoryConnection>();
            _units = dbContext.Set<Unit>();
            _unitRatios = dbContext.Set<IngredientUnitsRatio>();

            _ingredientProps = typeof(Ingredient)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _unitProps = typeof(Unit)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Guid> AddIngredient(AddIngredientDTO ingredientDTO)
        {
            if (_ingredients.Select(x => x.Name.ToLower()).Contains(ingredientDTO.Name.ToLower()))
            {
                throw new DuplicateIngredientException($"Ingredient with name {ingredientDTO.Name} already exists.");
            }

            var ingredient = new Ingredient()
            {
                Name = ingredientDTO.Name,
                Description = ingredientDTO.Description,
            };

            await _ingredients.AddAsync(ingredient);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug($"Ingredient {ingredientDTO.Name} added.");

            return ingredient.Id;
        }

        public async Task<Guid> AddIngredientWithCategoriesByNames(AddIngredientWithCategoryNamesDTO ingredientDTO)
        {
            // check if ingredient is a duplicate
            var nameLowercase = ingredientDTO.Name.ToLower();
            if (_ingredients.Select(x => x.Name.ToLower()).Contains(nameLowercase))
            {
                throw new DuplicateIngredientException($"Ingredient with name {ingredientDTO.Name} already exists.");
            }
            var categoriesNames = _categories.Select(x => x.Name.ToLower());
            var nonPresentCategories = ingredientDTO.IngredientCategories.Where(x => !categoriesNames.Contains(x.ToLower()));

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var categoriesList = new List<IngredientCategory>(nonPresentCategories.Count());

                foreach (var category in nonPresentCategories)
                {
                    var newCategory = new IngredientCategory()
                    {
                        Name = category,
                        Description = string.Empty
                    };

                    categoriesList.Add(newCategory);
                }

                var ingredient = new Ingredient()
                {
                    Name = ingredientDTO.Name,
                    Description = ingredientDTO.Description
                };

                await _ingredients.AddAsync(ingredient);

                foreach (var category in categoriesList)
                {
                    await _categories.AddAsync(category);
                }
                await _dbContext.SaveChangesAsync();

                // take all categories (present and non present) and connect them
                var categoriesIds = _categories.ToArray().Where(cat => ingredientDTO.IngredientCategories.Contains(cat.Name.ToLower())).Select(cat => cat.Id).ToList();

                var connections = new List<IngredientCategoryConnection>(categoriesIds.Count);

                foreach (var categoryId in categoriesIds)
                {
                    var connection = new IngredientCategoryConnection()
                    {
                        CategoryId = categoryId,
                        IngredientId = ingredient.Id
                    };
                    await _connections.AddAsync(connection);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return ingredient.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Issue with transaction {transaction.TransactionId}.\nException: {ex}.");
                throw;
            }
        }

        public Task<Guid> AddIngredientWithCategoriesByIds(AddIngredientWithCategoryIdsDTO ingredientDTO)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GetIngredientCategoryDTO> GetIngredientCategories(Guid ingredientId)
        {
            var categories = _connections
                .Where(connection => connection.IngredientId == ingredientId)
                .Include(connection => connection.IngredientCategory)
                .Select(connection => new GetIngredientCategoryDTO(
                    connection.IngredientCategory.Id,
                    connection.IngredientCategory.Name,
                    connection.IngredientCategory.Description))
                .ToArray();

            return categories;
        }

        public GetIngredientWithCategoriesDTO GetIngredientWithCategoriesById(Guid ingredientId)
        {
            var ingredient = _ingredients
                .Where(x => x.Id == ingredientId)
                .Include(x => x.Connections)
                .ThenInclude(x => x.IngredientCategory)
                .FirstOrDefault() ?? throw new IngredientNotFoundException($"Ingredient with id {ingredientId} not found.");

            return new GetIngredientWithCategoriesDTO(
                ingredientId, 
                ingredient.Name, 
                ingredient.Description ?? "",
                ingredient.Connections
                    .Select(x => x.Ingredient.Name)
                    .ToArray());
        }

        public async Task<PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>> GetAllIngredientCategories(int count, int page, bool orderByAsc, string sortBy, string query)
        {
            IQueryable<IngredientCategory> result;

            if (_ingredientProps.Contains(sortBy))
            {
                result = _categories.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                // by name by default
                result = orderByAsc ? _categories.OrderBy(x => x.Name) : _categories.OrderByDescending(x => x.Name);
            }

            // query
            result = result.Where(category => category.Name.Contains(query));

            // count
            int totalCount = await result.CountAsync();

            // project
            var data = await result
                .Where(category => category.Name.Contains(query))
                .Skip(page * count)
                .Take(count)
                .Select(category => new GetIngredientCategoryDTO(
                    category.Id,
                    category.Name,
                    category.Description ?? ""))
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

        public async Task<PaginatedResult<IEnumerable<GetIngredientWithCategoriesDTO>>> GetAllIngredientsWithCategories(int count, int page, bool orderByAsc, string sortBy, string query)
        {
            IQueryable<Ingredient> result;

            if (_ingredientProps.Contains(sortBy))
            {
                result = _ingredients.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                // by name by default
                result = orderByAsc ? _ingredients.OrderBy(x => x.Name) : _ingredients.OrderByDescending(x => x.Name);
            }

            // query
            result = result.Where(ingredient => ingredient.Name.Contains(query));

            // count the data 
            var totalCount = await result.CountAsync();

            // project
            var data = await result
                .Skip(page * count)
                .Take(count)
                .Include(ingredient => ingredient.Connections)
                .ThenInclude(connection => connection.IngredientCategory)
                .Select(ingredient => new GetIngredientWithCategoriesDTO(
                    ingredient.Id,
                    ingredient.Name,
                    ingredient.Description ?? "",
                    ingredient.Connections
                        .Select(connection => connection.IngredientCategory.Name)
                        .ToArray()))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetIngredientWithCategoriesDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public async Task<PaginatedResult<IEnumerable<GetIngredientDTO>>> GetAllIngredients(int count, int page, bool orderByAsc, string sortBy, string query)
        {
            IQueryable<Ingredient> result;

            if (_ingredientProps.Contains(sortBy))
            {
                result = _ingredients.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                // by name by default
                result = orderByAsc ? _ingredients.OrderBy(x => x.Name) : _ingredients.OrderByDescending(x => x.Name);
            }

            // queried
            result = result.Where(ingredient => ingredient.Name.Contains(query));

            // count the data
            var totalCount = await result.CountAsync();

            // project
            var data = await result
                .Skip(page * count)
                .Take(count)
                .Include(ingredient => ingredient.Connections)
                .ThenInclude(connection => connection.IngredientCategory)
                .Select(ingredient => new GetIngredientDTO(
                    ingredient.Id,
                    ingredient.Name,
                    ingredient.Description ?? ""))
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

        public GetIngredientDTO GetIngredientById(Guid ingredientId)
        {
            var ingredient = _ingredients
                .FirstOrDefault(r => r.Id == ingredientId) ?? throw new RecipeNotFoundException($"Recipe with id {ingredientId} not found.");

            return new GetIngredientDTO(ingredientId, ingredient.Name, ingredient.Description ?? "");
        }

        public async Task<Guid> AddIngredientCategory(AddIngredientCategoryDTO ingredientDTO)
        {
            if (_categories.Any(x => x.Name == ingredientDTO.Name))
            {
                throw new DuplicateIngredientCategoryException($"Ingredient with name {ingredientDTO.Name} already exists.");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var ingredientCategory = new IngredientCategory()
                {
                    Name = ingredientDTO.Name,
                    Description = ingredientDTO.Description
                };

                await _categories.AddAsync(ingredientCategory);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return ingredientCategory.Id;
            }
            catch  
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public GetUnitDTO GetUnit(Guid unitId)
        {
            var unit = _units.FirstOrDefault(x => x.Id == unitId) ?? throw new UnitNotFoundException($"Unit with id {unitId} not found.");

            return new GetUnitDTO(unitId, unit.Name);
        }

        public async Task<PaginatedResult<IEnumerable<GetUnitDTO>>> GetAllUnits(int count, int page, bool orderByAsc, string sortBy, string query)
        {
            // query
            var units = string.IsNullOrEmpty(query)
                ? _units
                : _units.Where(recipe => recipe.Name.Contains(query));

            // sorting
            if (_unitProps.Contains(sortBy))
            {
                units = units.OrderBy(sortBy, orderByAsc);
            }
            else
            {
                units = orderByAsc ? units.OrderBy(r => r.Name) : units.OrderByDescending(r => r.Name);
            }

            // count
            int totalCount = await units.CountAsync();

            // project
            var data = await units
                .Skip(page * count)
                .Take(count)
                .Select(recipe => new GetUnitDTO(
                    recipe.Id,
                    recipe.Name))
                .ToListAsync();

            return new PaginatedResult<IEnumerable<GetUnitDTO>>
            {
                Data = data,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / count),
                Page = page,
                PageSize = count
            };
        }

        public GetTranslatedUnitQuantitiesDTO GetTranslatedUnitQuantities(RequestUnitQuantityTranslationDTO dto)
        {
           var ratio = _unitRatios
                .Where(x => x.IngredientId == dto.IngredientId)
                .Where(x => x.UnitOneId == dto.UnitOneId || x.UnitTwoId == dto.UnitOneId)
                .Include(x => x.Ingredient)
                .Include(x => x.UnitOne)
                .Include(x => x.UnitTwo)
                .FirstOrDefault(x => x.UnitTwoId == dto.UnitTwoId || x.UnitOneId == dto.UnitTwoId) 
                ?? throw new UnitTranslationNotFoundException($"Unit translation for ingredient with id {dto.IngredientId} does not exist for units with ids {dto.UnitOneId} and {dto.UnitTwoId} not found.");

            var finalQuantity = dto.Quantity;
            
            if (ratio.UnitOneId == dto.UnitOneId)
            {
                finalQuantity = Math.Round(dto.Quantity * ratio.OneToTwoRatio, 3);

                return new GetTranslatedUnitQuantitiesDTO(
                    ratio.IngredientId, 
                    ratio.Ingredient.Name, 
                    ratio.UnitOneId, 
                    ratio.UnitOne.Name, 
                    ratio.UnitTwoId, 
                    ratio.UnitTwo.Name,
                    dto.Quantity,
                    finalQuantity);
            }

            finalQuantity = Math.Round(dto.Quantity * (1/ratio.OneToTwoRatio), 3);

            return new GetTranslatedUnitQuantitiesDTO(
                ratio.IngredientId, 
                ratio.Ingredient.Name,
                ratio.UnitTwoId,
                ratio.UnitTwo.Name,
                ratio.UnitOneId,
                ratio.UnitOne.Name,
                dto.Quantity,
                finalQuantity
                );
        }
    }
}
