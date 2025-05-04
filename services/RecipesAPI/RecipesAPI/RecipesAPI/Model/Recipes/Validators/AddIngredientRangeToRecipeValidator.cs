using FluentValidation;
using RecipesAPI.Model.Recipes.Add;

namespace RecipesAPI.Model.Recipes.Validators
{
    public class AddIngredientRangeToRecipeValidator : AbstractValidator<AddIngredientRangeToRecipeDTO>
    {
        public AddIngredientRangeToRecipeValidator()
        {

            RuleFor(x => x.IngredientIds)
                .NotEmpty()
                .WithMessage("No ingredient ids were passed in the object.");
        }
    }
}
