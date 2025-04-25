namespace RecipesAPI.Exceptions.Duplicates
{
    public class DuplicateRecipeException : DuplicateElementException
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
