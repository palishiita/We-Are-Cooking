namespace RecipesAPI.Exceptions.Duplicates
{
    public class DuplicateElementException : Exception
    {
        public DuplicateElementException()
        {
        }

        public DuplicateElementException(string? message) : base(message)
        {
        }

        public DuplicateElementException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
