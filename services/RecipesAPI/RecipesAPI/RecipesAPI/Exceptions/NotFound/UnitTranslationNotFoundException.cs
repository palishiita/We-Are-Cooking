
namespace RecipesAPI.Exceptions.NotFound
{
    public class UnitTranslationNotFoundException : ElementNotFoundException
    {
        public UnitTranslationNotFoundException()
        {
        }

        public UnitTranslationNotFoundException(string? message) : base(message)
        {
        }

        public UnitTranslationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
