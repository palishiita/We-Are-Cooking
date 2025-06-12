namespace RecipesAPI.Exceptions.NotFound
{
    public class RecipeNotFoundException : ElementNotFoundException
    {
        public RecipeNotFoundException()
        {
        }

        public RecipeNotFoundException(string? message) : base(message)
        {
        }

        public RecipeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
