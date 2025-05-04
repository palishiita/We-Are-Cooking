
namespace RecipesAPI.Exceptions.NotFound
{
    public class CookbookRecipeNotFound : ElementNotFoundException
    {
        public CookbookRecipeNotFound()
        {
        }

        public CookbookRecipeNotFound(string? message) : base(message)
        {
        }

        public CookbookRecipeNotFound(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
