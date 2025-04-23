namespace RecipesAPI.Exceptions
{
    public class DuplicateRecipeException : Exception
    {
        public DuplicateRecipeException()
        {
        }

        public DuplicateRecipeException(string? message) : base(message)
        {
        }

        public DuplicateRecipeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
