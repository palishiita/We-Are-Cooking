namespace RecipesAPI.Exceptions
{
    public class RecipeNotFoundException : Exception
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
