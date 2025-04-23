namespace RecipesAPI.Exceptions
{
    public class DuplicateIngredientException : Exception
    {
        public DuplicateIngredientException()
        {
        }

        public DuplicateIngredientException(string? message) : base(message)
        {
        }

        public DuplicateIngredientException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
