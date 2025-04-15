namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// A representation of an ingredient in the fridge of a user.
    /// </summary>
    public class UserFridge
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }
        public Guid UserId { get; set; }
    }
}
