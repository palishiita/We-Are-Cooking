namespace RecipesAPI.Exceptions.NotFound
{
    public class IngredientNotFoundException : ElementNotFoundException
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