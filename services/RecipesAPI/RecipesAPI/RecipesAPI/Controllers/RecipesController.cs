using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Database;
using RecipesAPI.Services.Interfaces;

namespace RecipesAPI.Controllers
{
    [Route("recipesapi/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeDbContext recipeDbContext;
        
        IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService, RecipeDbContext recipeDb)
        {
            recipeDbContext = recipeDb;
            _recipeService = recipeService;
        }

        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            if (recipeDbContext.Database.CanConnect())
            {
                return Ok("Workiiiing :3");
            }
            return NotFound("Not working.... :----(");
        }

    }
}
