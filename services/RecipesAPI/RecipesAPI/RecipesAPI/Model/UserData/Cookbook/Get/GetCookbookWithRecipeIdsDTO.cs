﻿namespace RecipesAPI.Model.UserData.Cookbook.Get
{
    public record GetCookbookWithRecipeIdsDTO(Guid Id, Guid UserId, IEnumerable<Guid> RecipeIds);
}
