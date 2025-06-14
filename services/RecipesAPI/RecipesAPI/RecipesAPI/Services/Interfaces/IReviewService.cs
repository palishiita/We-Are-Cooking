﻿using RecipesAPI.Model.Common;
using RecipesAPI.Model.Reviews.Add;
using RecipesAPI.Model.Reviews.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<GetReviewDTO?> GetReviewById(Guid reviewId, Guid requestedUserId, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetReviewDTO>>> GetReviewsByUserId(Guid userId, int pageNumber, int pageSize, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetReviewDTO>>> GetReviewsByRecipeId(Guid recipeId, CancellationToken ct, int pageNumber, int pageSize);
        Task<Guid> AddReview(AddReviewRequestDTO dto, Guid userId, Guid recipeId, CancellationToken ct);
        Task DeleteReview(Guid recipeId, Guid userId, CancellationToken ct);
    }
}
