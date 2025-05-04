using FluentValidation;
using RecipesAPI.Model.Recipes.Add;

namespace RecipesAPI.Model.Recipes.Validators
{
    public class AddRecipeWithIngredientIdsValidator : AbstractValidator<AddRecipeWithIngredientIdsDTO>
    {
        public AddRecipeWithIngredientIdsValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Name is required and should have at most 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description should have at most 2000 characters.");

            RuleFor(x => x.IngredientIds)
                .NotEmpty()
                .WithMessage("No ingredient ids were passed in the object.");
        }
    }
}
