
namespace RecipesAPI.Exceptions.Duplicates
{
    public class DuplicateIngredientCategoryException : DuplicateElementException
    {
        public DuplicateIngredientCategoryException()
        {
        }

        public DuplicateIngredientCategoryException(string? message) : base(message)
        {
        }

        public DuplicateIngredientCategoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
