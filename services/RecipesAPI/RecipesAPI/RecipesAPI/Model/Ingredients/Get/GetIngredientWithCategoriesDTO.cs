﻿namespace RecipesAPI.Model.Ingredients.Get
{
    public record GetIngredientWithCategoriesDTO(Guid Id, string Name, string Description, IEnumerable<GetIngredientCategoryDTO> Categories);
}
