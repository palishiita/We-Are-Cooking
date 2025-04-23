using System.Runtime.Serialization;

namespace RecipesAPI.Exceptions
{
    internal class IngredientNotFoundException : Exception
    {
        public IngredientNotFoundException()
        {
        }

        public IngredientNotFoundException(string? message) : base(message)
        {
        }

        public IngredientNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}