using FluentValidation;
using RecipesAPI.Model.Recipes.Update;

namespace RecipesAPI.Model.Recipes.Validators
{
    public class UpdateRecipeValidator : AbstractValidator<UpdateRecipeDTO>
    {
        public UpdateRecipeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Name is required and should have at most 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description should have at most 2000 characters.");
        }
    }
}
