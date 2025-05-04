namespace RecipesAPI.Exceptions.Duplicates
{
    public class DuplicateIngredientException : DuplicateElementException
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
