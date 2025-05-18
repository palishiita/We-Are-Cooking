
namespace RecipesAPI.Exceptions.NotFound
{
    public class UnitNotFoundException : ElementNotFoundException
    {
        public UnitNotFoundException()
        {
        }

        public UnitNotFoundException(string? message) : base(message)
        {
        }

        public UnitNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
